using Digi.Example_NetworkProtobuf;
using Sandbox.ModAPI;
using VRage.Game.Components;

namespace GVA.NPCControl.Client
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class ClientSession: MySessionComponentBase
    {
        public static Client client = null;

        public override void BeforeStart()
        {
            if (!MyAPIGateway.Utilities.IsDedicated)
            {
                if (client == null) client = new Client(Networking.ModLast4);
            }
        }

        protected override void UnloadData()
        {
            if (client != null) client.Unload();
            client = null;
        }
    }
}
