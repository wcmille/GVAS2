using System.Collections.Generic;

namespace GVA.NPCControl.Client
{
    public class ClientWorld : SharedWorld
    {
        readonly Client client;
        readonly ClientPirateAccount black;

        public ClientWorld(Client client)
        {
            var blue = new ClientAccount(SharedConstants.BlueFactionColor);
            list.Add(blue);
            var red = new ClientAccount(SharedConstants.RedFactionColor);
            list.Add(red);
            black = new ClientPirateAccount();
            list.Add(black);

            this.client = client;
        }

        public void GetAccountByPCOwner(string factionTag, List<IAccount> accounts)
        {
            accounts.Clear();
            foreach (IAccount f in list)
            {
                if (f.OwningPCTag == factionTag) accounts.Add(f);
            }
            if (accounts.Count == 0) accounts.Add(black);
        }

        public override void RequestReport(ulong requestor, IAccount ac)
        {
            client.Send(new CommandPacket(ac.OwningPCTag, ac.ColorFaction, SharedConstants.ReportCommand));
        }
    }
}
