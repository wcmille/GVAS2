using Digi.Example_NetworkProtobuf;
using System;

namespace GVA.NPCControl.Client
{
    public class Client : IPacketReceiver
    {
        Networking networking;

        public Client(ushort channel)
        {
            networking = new Networking(channel, this);
            networking.Register();
        }

        public void Received(PacketBase packet)
        {
            packet.Execute();
        }

        internal void Unload()
        {
            networking?.Unregister();
            networking = null;
        }

        internal void Send(PacketBase packet)
        {
            networking.SendToServer(packet);
        }
    }
}
