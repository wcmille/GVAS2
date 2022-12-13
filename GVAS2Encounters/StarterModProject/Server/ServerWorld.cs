using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;

namespace GVA.NPCControl.Server
{
    public class ServerWorld : SharedWorld
    {
        readonly Server server;
        readonly IMyCubeGrid blueClaimBlock;
        readonly IMyCubeGrid redClaimBlock;
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
                var ownerID = blueClaimBlock.BigOwners.First();
                var blueFaction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(ownerID);
                var blueTag = blueFaction.Tag;
                MyAPIGateway.Utilities.SetVariable($"{color}{SharedConstants.OwnerStr}", blueTag);
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
            foreach (var acct in list)
            {
                acct.TimePeriod();
                Write(acct);
            }
        }

        //MyLog.Default.WriteLine("SATDISH: UpdateOnceStart - !IsDedicated");
        //names.Clear();
        //MyAPIGateway.Entities.GetEntities(names, x => x is IMyCubeGrid && x.DisplayName != null);
        //if (names.Select(x => x.DisplayName.Contains("Faction Territory Blue")).Count() == 0)
        //{
        //    MyLog.Default.WriteLine("SATDISH: No Territory Grids Found.");
        //}
        //else
        //{
        //    MyLog.Default.WriteLine("SATDISH: Territory Grid Found.");
        //    foreach (var item in names)
        //    {
        //        if (!item.DisplayName.Contains("Faction Territory Blue")) continue;
        //        var beaconBlock = item as MyCubeGrid;
        //        var beacons = ((IMyCubeGrid)beaconBlock).GetFatBlocks<IMyBeacon>();
        //        if (beacons.Count() != 1) continue;
        //        var beacon = beacons.First();
        //        jaghFactionBlock = (MyCubeGrid)item;
        //    }
        //}


        //if (jaghFactionBlock == null)
        //{
        //    return MyAPIGateway.Session.Factions.TryGetFactionByTag(SharedConstants.BlackFactionTag);
        //}
        //else
        //{
        //    var owner = jaghFactionBlock.BigOwners.First();
        //    var supportedfaction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(owner);
        //    return supportedfaction;
        //}


        public override void Write(Accounting acct)
        {
            acct.Write();
            server.WriteToClient(acct);
        }
    }
}
