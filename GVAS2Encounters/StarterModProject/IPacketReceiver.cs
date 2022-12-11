using Digi.Example_NetworkProtobuf;

namespace GVA.NPCControl
{
    public interface IPacketReceiver
    {
        void Received(PacketBase packet);
    }
}
