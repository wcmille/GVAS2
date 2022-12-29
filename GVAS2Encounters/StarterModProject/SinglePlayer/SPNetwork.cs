using Digi.Example_NetworkProtobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GVA.NPCControl.SinglePlayer
{
    public class SPNetwork : INetworking
    {
        Action<PacketBase> receiver;

        public SPNetwork(Action<PacketBase> receiver)
        {
            this.receiver = receiver;
        }

        public void Register()
        {
            //Do nothing.
        }

        public void RelayToClients(PacketBase packet, byte[] rawData = null)
        {
            receiver(packet);
        }

        public void SendToPlayer(PacketBase packet, ulong steamId)
        {
            receiver(packet);
        }

        public void SendToServer(PacketBase packet)
        {
            receiver(packet);
        }

        public void Unregister()
        {
            //Do nothing.
        }
    }
}
