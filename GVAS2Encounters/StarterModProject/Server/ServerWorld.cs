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
        readonly IMyCubeGrid blueClaimBlock;
        //readonly IMyCubeGrid redClaimBlock;
        readonly HashSet<IMyEntity> names = new HashSet<IMyEntity>(1);

        public ServerWorld(Server server)
        {
            this.server = server;
            FetchClaimBlocks();
            var color = SharedConstants.BlueFactionColor;
            blueClaimBlock = GetClaimBlock(color);
            if (blueClaimBlock == null) MyLog.Default.WriteLine("SATDISH: {color} Territory Grid Not Found.");
            else
            {
                WriteOwner(color);
                blueClaimBlock.OnBlockOwnershipChanged += BlueClaimBlock_OnBlockOwnershipChanged;
            }
        }

        public void Dispose()
        {
            blueClaimBlock.OnBlockOwnershipChanged -= BlueClaimBlock_OnBlockOwnershipChanged;
        }

        private void WriteOwner(string color)
        {
            var ownerID = blueClaimBlock.BigOwners.First();
            var blueFaction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(ownerID);
            var blueTag = blueFaction.Tag;
            //MyLog.Default.WriteLineAndConsole($"SATDISH: Owners: {ownerID} {blueFaction.Name} {blueTag}");
            MyAPIGateway.Utilities.SetVariable($"{color}{SharedConstants.OwnerStr}", blueTag);
        }

        private void BlueClaimBlock_OnBlockOwnershipChanged(IMyCubeGrid obj)
        {
            var color = SharedConstants.BlueFactionColor;
            WriteOwner(color);
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

        private IMyCubeGrid GetClaimBlock(string color)
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
