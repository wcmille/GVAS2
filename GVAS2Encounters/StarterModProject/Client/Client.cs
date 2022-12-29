using Digi.Example_NetworkProtobuf;

namespace GVA.NPCControl.Client
{
    public class Client
    {
        public static Client client = null;

        INetworking networking;
        public IWorld World { get; set; }

        public Client()
        {
            if (client == null) client = this;
        }

        public void BeforeStart(INetworking networking)
        {
            World = new ClientWorld(this);
            this.networking = networking;
            networking.Register();
        }

        internal void UnloadData()
        {
            networking?.Unregister();
            networking = null;
        }

        internal void Send(PacketBase packet)
        {
            networking.SendToServer(packet);
        }

        public void Received(PacketBase packet)
        {
            packet.Execute(World);
        }
    }
}
