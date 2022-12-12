using Digi.Example_NetworkProtobuf;
using System.Collections.Generic;

namespace GVA.NPCControl.Server
{
    public class World : IWorld
    {
        List<Accounting> list = new List<Accounting>();
        Server server;

        public World(Server server)
        {
            this.server = server;
            list.Add(new Accounting("Blue"));
        }

        public Accounting GetAccountDetails(string color)
        {
            return list[0];
        }

        public void Write(Accounting acct)
        {
            acct.Write();
            server.WriteToClient(acct);
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
            world = new World(this);
        }

        internal void Unload()
        {
            networking?.Unregister();
            networking = null;
            world = null;
        }

        public void WriteToClient(Accounting acct)
        {
            FactionValuesPacket packet = new FactionValuesPacket(acct.ColorFaction, acct.Civilian, acct.Military, acct.UnspentUnits);
            networking.RelayToClients(packet);
        }

        public void Received(PacketBase packet)
        {
            packet.Execute(world);
        }
    }
}
