using System;

namespace GVA.NPCControl.Client
{
    public class ClientWorld : SharedWorld
    {
        readonly Client client;

        public ClientWorld(Client client)
        {
            this.client = client;
        }

        public override void RequestReport(ulong requestor, IAccount ac)
        {
            client.Send(new CommandPacket(ac.OwningPCTag, ac.ColorFaction, SharedConstants.ReportCommand));
        }
    }
}
