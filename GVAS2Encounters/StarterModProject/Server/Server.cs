using Digi.Example_NetworkProtobuf;

namespace GVA.NPCControl.Server
{
    public class Server
    {
        Networking networking;

        public Server(ushort channel, IPacketReceiver receiver)
        {
            networking = new Networking(channel, receiver);
            networking.Register();
        }

        internal void Unload()
        {
            networking?.Unregister();
            networking = null;
            //world = null;
        }

        public void WriteToClient(ulong client, ServerLog log)
        {
            var packet = new DialogResponsePacket(log.LogText);
            networking.SendToPlayer(packet, client);
        }

        public void WriteToClient(IAccount acct)
        {
            FactionValuesPacket packet = new FactionValuesPacket(acct.OwningPCTag, acct.ColorFaction, acct.Civilian, acct.Military, acct.UnspentUnits);
            networking.RelayToClients(packet);
        }
    }
}
