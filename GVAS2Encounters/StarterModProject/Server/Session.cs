using Digi.Example_NetworkProtobuf;
using Sandbox.ModAPI;
using VRage.Game.Components;

namespace GVA.NPCControl.Server
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class Session : MySessionComponentBase
    {
        Server server = null;

        public override void BeforeStart()
        {
            if (MyAPIGateway.Multiplayer.IsServer)
            {
                server = new Server(Networking.ModLast4);
                Time();
            }
        }

        protected override void UnloadData()
        {
            if (server != null) server.Unload();
        }

        private void Time()
        {
            string faction = "Blue";
            Accounting acct = new Accounting(faction);

            acct.TimePeriod();

            acct.Write();
            server.WriteToClient(acct);
        }
    }
}
