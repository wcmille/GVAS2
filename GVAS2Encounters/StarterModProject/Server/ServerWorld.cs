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
