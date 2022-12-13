using Digi.Example_NetworkProtobuf;
using Sandbox.ModAPI;
using VRage.Game.Components;

namespace GVA.NPCControl.Server
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class Session : MySessionComponentBase
    {
        private Server server = null;
        private ServerWorld world = null;

        public override void BeforeStart()
        {
            if (MyAPIGateway.Multiplayer.IsServer)
            {
                server = new Server(Networking.ModLast4);
                world = new ServerWorld(server);
                world.Time();
            }
        }

        protected override void UnloadData()
        {
            if (server != null) server.Unload();
        }
    }
}
