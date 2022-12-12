using System.Collections.Generic;

namespace GVA.NPCControl.Server
{
    public class ServerWorld : IWorld
    {
        readonly List<Accounting> list = new List<Accounting>();
        readonly Server server;

        public ServerWorld(Server server)
        {
            this.server = server;
            list.Add(new Accounting("Blue"));
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


        public Accounting GetAccountDetails(string color)
        {
            return list[0];
        }

        public Accounting GetTerritoryOwner(string factionTag)
        {
            foreach (var f in list)
            {
                if (f.OwningPCTag == factionTag) return f;
            }
            return null;
        }

        public void Write(Accounting acct)
        {
            acct.Write();
            server.WriteToClient(acct);
        }
    }
}
