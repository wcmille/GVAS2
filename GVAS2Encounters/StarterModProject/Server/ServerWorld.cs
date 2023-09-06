using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;

namespace GVA.NPCControl.Server
{
    public class ServerWorld : SharedWorld, IDisposable
    {
        readonly Server server;
        IMyCubeGrid blueClaimBlock;
        IMyCubeGrid redClaimBlock;
        private readonly ServerLog blueLog;
        private readonly ServerLog redLog;
        readonly HashSet<IMyEntity> names = new HashSet<IMyEntity>(1);
        readonly PirateAccount black;

        public ServerWorld(Server server)
        {
            blueLog = new ServerLog(SharedConstants.BlueFactionColor);
            redLog = new ServerLog(SharedConstants.RedFactionColor);
            blueLog.Read();
            redLog.Read();

            var blue = new ServerAccount(SharedConstants.BlueFactionColor, blueLog);
            list.Add(blue);
            var red = new ServerAccount(SharedConstants.RedFactionColor, redLog);
            list.Add(red);
            black = new PirateAccount();
            list.Add(black);

            this.server = server;
            FetchClaimBlocks();
            blueClaimBlock = GetClaimBlockByColor(SharedConstants.BlueFactionColor);
            redClaimBlock = GetClaimBlockByColor(SharedConstants.RedFactionColor);

            MyAPIGateway.Session.Factions.FactionStateChanged += Factions_FactionStateChanged;
        }

        internal void Factions_FactionStateChanged(MyFactionStateChange action, long fromFactionId, long toFactionId, long playerId, long senderId)
        {
            foreach (var faction in list)
            {
                ServerAccount sf = faction as ServerAccount;
                if (sf != null && sf.OwningPCFaction != null)
                {
                    //Only get here if PlayerFaction, owned by a PC
                    if (action == MyFactionStateChange.FactionMemberAcceptJoin)
                    {
                        var toFaction = MyAPIGateway.Session.Factions.TryGetFactionById(toFactionId);
                        sf.PlayerJoined(playerId, toFaction);
                    }
                    //Here if the faction change includes one of the owning factions.
                    else if (sf.OwningPCFaction.FactionId == fromFactionId || sf.OwningPCFaction.FactionId == toFactionId)
                    {
                        if (action == MyFactionStateChange.FactionMemberLeave || action == MyFactionStateChange.FactionMemberKick)
                        {
                            sf.PlayerLeft(playerId);
                        }
                        else if (action == MyFactionStateChange.RemoveFaction)
                        {
                            //Ask red and blue fact: Did PC faction just poof?
                            sf.FactionLeft();
                        }
                        else if (action == MyFactionStateChange.AcceptPeace)
                        {
                            sf.Peace(fromFactionId ^ toFactionId ^ sf.OwningPCFaction.FactionId);
                        }
                        else if (action == MyFactionStateChange.DeclareWar)
                        {
                            sf.War(fromFactionId ^ toFactionId ^ sf.OwningPCFaction.FactionId);
                        }
                    }
                }
            }
        }

        private IMyCubeGrid GetClaimBlockByColor(string color)
        {
            var claimBlockGrid = GetClaimBlockGrid(color);
            if (claimBlockGrid == null) MyLog.Default.WriteLine("SATDISH: {color} Territory Grid Not Found.");
            else
            {
                WriteOwner(claimBlockGrid, color);
                claimBlockGrid.OnBlockOwnershipChanged += ClaimBlock_OnBlockOwnershipChanged;
            }

            return claimBlockGrid;
        }

        public void Dispose()
        {
            MyAPIGateway.Session.Factions.FactionStateChanged -= Factions_FactionStateChanged;
            if (blueClaimBlock != null)
            {
                blueClaimBlock.OnBlockOwnershipChanged -= ClaimBlock_OnBlockOwnershipChanged;
                blueClaimBlock = null;
            }
            if (redClaimBlock != null)
            {
                redClaimBlock.OnBlockOwnershipChanged -= ClaimBlock_OnBlockOwnershipChanged;
                redClaimBlock = null;
            }
        }

        public override void Write(IAccount acct)
        {
            if (acct == null) return;
            base.Write(acct);
            if (acct is Accounting) server.WriteToClient(acct as Accounting);
        }

        public override void RequestReport(ulong requestor, IAccount ac)
        {
            var sa = ac as ServerAccount;
            var log = sa?.AccountLog;
            if (log != null) server.WriteToClient(requestor, log);
        }

        #region Private Methods

        private void WriteOwner(IMyCubeGrid claimBlockGrid, string color)
        {
            var ownerID = claimBlockGrid.BigOwners.First();
            var faction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(ownerID);
            var factionId = faction.FactionId;
            //MyLog.Default.WriteLineAndConsole($"SATDISH: Owners: {ownerID} {blueFaction.Name} {blueTag}");
            MyAPIGateway.Utilities.SetVariable($"{color}{SharedConstants.OwnerId}", factionId);
        }

        private void ClaimBlock_OnBlockOwnershipChanged(IMyCubeGrid obj)
        {
            string color;
            if (obj.CustomName.Contains(SharedConstants.BlueFactionColor)) color = SharedConstants.BlueFactionColor;
            else if (obj.CustomName.Contains(SharedConstants.RedFactionColor)) color = SharedConstants.RedFactionColor;
            else
            {
                MyLog.Default.WriteLine($"SATDISH: Couldn't determine color of territory marker.");
                return;
            }

            WriteOwner(obj, color);
            var acct = GetAccountByColor(color);
            acct.Read();
            Write(acct);

            var faction = MyAPIGateway.Session.Factions.TryGetFactionByTag(acct.OwningNPCTag);

            var identities = new List<IMyIdentity>();
            int defaultRep = SharedConstants.DefaultRep;
            MyAPIGateway.Players.GetAllIdentites(identities);

            foreach (var identity in identities)
            {
                defaultRep = SharedConstants.DefaultRep;
                ulong steamId = MyAPIGateway.Players.TryGetSteamId(identity.IdentityId);

                if (steamId > 0)
                {
                    var playerFaction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(identity.IdentityId);
                    if (playerFaction != null && playerFaction.Tag == acct.OwningPCTag) defaultRep = SharedConstants.AlliedRep;
                    MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(identity.IdentityId, faction.FactionId, defaultRep);
                }
            }
        }

        private void FetchClaimBlocks()
        {
            names.Clear();
            MyAPIGateway.Entities.GetEntities(names, x => x is IMyCubeGrid && x.DisplayName != null && x.DisplayName.Contains($"Faction Territory"));

            if (names.Count == 0)
            {
                MyLog.Default.WriteLine("SATDISH: No Territory Grids Found.");
            }
        }

        private IMyCubeGrid GetClaimBlockGrid(string color)
        {
            foreach (var e in names)
            {
                var grid = (IMyCubeGrid)e;
                var blocks = grid.GetFatBlocks<IMyBeacon>();
                if (blocks.Count() != 1) continue;
                if (grid.CustomName.Contains($"Faction Territory {color}")) return grid;
            }
            return null;
        }

        /// <summary>
        /// Run each restart.
        /// </summary>
        internal void Time()
        {
            DateTime lastRun = ReadTime();
            var diff = DateTime.Now - lastRun;
            if (diff.Hours > SharedConstants.TimeDeltaHours)
            {
                foreach (IServerAccount acct in list)
                {
                    //ServerLog factionLog = null;
                    //if (acct.ColorFaction == SharedConstants.BlueFactionColor) factionLog = blueLog;
                    //else if (acct.ColorFaction == SharedConstants.RedFactionColor) factionLog = redLog;
                    //factionLog?.Log(acct);
                    acct.TimePeriod(black);
                    Write(acct);
                }
                var newTime = lastRun.AddHours(SharedConstants.TimeDeltaHours);
                WriteTime(newTime);
            }
        }

        private void WriteTime(DateTime newTime)
        {
            MyAPIGateway.Utilities.SetVariable($"{SharedConstants.ModNamespace}.{SharedConstants.TimeStr}", newTime);
        }

        private DateTime ReadTime()
        {
            DateTime time;
            MyAPIGateway.Utilities.GetVariable($"{SharedConstants.ModNamespace}.{SharedConstants.TimeStr}", out time);
            if (time == DateTime.MinValue)
            {
                time = DateTime.Now;
                WriteTime(time);
            }
            return time;
        }
        #endregion
    }
}
