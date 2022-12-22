using Digi.Example_NetworkProtobuf;

namespace GVA.NPCControl.Server
{
    public class Server : IPacketReceiver
    {
        Networking networking;
        ServerWorld world;

        public Server(ushort channel)
        {
            networking = new Networking(channel, this);
            networking.Register();
            world = new ServerWorld(this);
        }

        internal void Unload()
        {
            networking?.Unregister();
            networking = null;
            world.Dispose();
            world = null;
        }

        public void WriteToClient(IAccount acct)
        {
            FactionValuesPacket packet = new FactionValuesPacket(acct.OwningPCTag, acct.ColorFaction, acct.Civilian, acct.Military, acct.UnspentUnits);
            networking.RelayToClients(packet);
        }

        public void Received(PacketBase packet)
        {
            packet.Execute(world);
        }
    }
}
