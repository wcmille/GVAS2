using Digi.Example_NetworkProtobuf;
using System.Collections.Generic;

namespace GVA.NPCControl.Server
{
    public class World : IWorld
    {
        List<Accounting> list = new List<Accounting>();

        public World()
        {
            list.Add(new Accounting("JAGH"));
        }

        public Accounting GetAccountDetails(string color)
        {
            return list[0];
        }
    }

    public class Server : IPacketReceiver
    {
        Networking networking;
        IWorld world;

        public Server(ushort channel)
        {
            networking = new Networking(channel, this);
            networking.Register();
            world = new World();
        }

        internal void Unload()
        {
            networking?.Unregister();
            networking = null;
            world = null;
        }

        public void WriteToClient(Accounting acct)
        {
            FactionValuesPacket packet = new FactionValuesPacket(acct.Faction, acct.Civilian, acct.Military, acct.UnspentUnits);
            networking.RelayToClients(packet);
        }

        public void Received(PacketBase packet)
        {
            packet.Execute(world);
        }
    }
}
