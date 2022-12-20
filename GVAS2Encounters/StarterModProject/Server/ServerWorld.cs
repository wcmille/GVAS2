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
        readonly HashSet<IMyEntity> names = new HashSet<IMyEntity>(1);

        public ServerWorld(Server server)
        {
            this.server = server;
            FetchClaimBlocks();
            blueClaimBlock = GetClaimBlockByColor(SharedConstants.BlueFactionColor);
            redClaimBlock = GetClaimBlockByColor(SharedConstants.RedFactionColor);
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

        private void WriteOwner(IMyCubeGrid claimBlockGrid, string color)
        {
            var ownerID = claimBlockGrid.BigOwners.First();
            var faction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(ownerID);
            var tag = faction.Tag;
            //MyLog.Default.WriteLineAndConsole($"SATDISH: Owners: {ownerID} {blueFaction.Name} {blueTag}");
            MyAPIGateway.Utilities.SetVariable($"{color}{SharedConstants.OwnerStr}", tag);
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
            int defaultRep = 0;
            MyAPIGateway.Players.GetAllIdentites(identities);

            foreach (var identity in identities)
            {
                defaultRep = 0;
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

        internal void Time()
        {
            DateTime lastRun = ReadTime();
            var diff = DateTime.Now - lastRun;
            if (diff.Hours > SharedConstants.TimeDeltaHours)
            {
                foreach (var acct in list)
                {
                    acct.TimePeriod();
                    Write(acct);
                }
                var newTime = lastRun.AddHours(SharedConstants.TimeDeltaHours);
                WriteTime(newTime);
            }
        }

        private void WriteTime(DateTime newTime)
        {
            MyAPIGateway.Utilities.SetVariable($"{SharedConstants.TimeStr}", newTime);
        }

        private DateTime ReadTime()
        {
            DateTime time;
            MyAPIGateway.Utilities.GetVariable($"{SharedConstants.TimeStr}", out time);
            if (time == DateTime.MinValue) time = DateTime.Now;
            return time;
        }

        public override void Write(Accounting acct)
        {
            if (acct == null) return;
            base.Write(acct);            
            server.WriteToClient(acct);
        }
    }
}
