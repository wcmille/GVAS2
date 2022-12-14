using Sandbox.ModAPI;
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
                WriteOwner(color);
                blueClaimBlock.OnBlockOwnershipChanged += BlueClaimBlock_OnBlockOwnershipChanged;
            }
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
            //MyLog.Default.WriteLine($"SATDISH: Owner Changed. {obj.CustomName}");
            var color = SharedConstants.BlueFactionColor;
            WriteOwner(color);
            var acct = GetAccountByColor(color);
            acct.Read();
            Write(acct);
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

        public override void Write(Accounting acct)
        {
            if (acct == null) return;
            base.Write(acct);            
            server.WriteToClient(acct);
        }
    }
}
