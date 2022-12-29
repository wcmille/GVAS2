using Digi.Example_NetworkProtobuf;
using GVA.NPCControl.Server;
using GVA.NPCControl.SinglePlayer;
using Sandbox.ModAPI;
using VRage.Game.Components;

namespace GVA.NPCControl.SE
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class Session : MySessionComponentBase
    {
        ServerSession serverS;
        Client.Client clientS;

        public override void LoadData()
        {
            if (MyAPIGateway.Utilities.IsDedicated) //Server
            {
                serverS = new ServerSession();
                clientS = null;
            }
            else if (MyAPIGateway.Multiplayer.IsServer) //SP
            {
                serverS = new ServerSession();
                clientS = new Client.Client();
            }
            else
            {
                serverS = null;
                clientS = new Client.Client();
            }
        }

        public override void BeforeStart()
        {
            if (MyAPIGateway.Utilities.IsDedicated) //Server
            {
                INetworking network = new Networking(Networking.ModLast4, serverS.Received);
                serverS.BeforeStart(network);
                clientS = null;
            }
            else if (MyAPIGateway.Multiplayer.IsServer) //SP
            {
                INetworking callServer = new SPNetwork(serverS.Received);
                clientS.BeforeStart(callServer);

                INetworking callClient = new SPNetwork(clientS.Received);
                serverS.BeforeStart(callClient);
            }
            else //Client
            {
                INetworking network = new Networking(Networking.ModLast4, clientS.Received);
                clientS.BeforeStart(network);
            }
        }

        protected override void UnloadData()
        {
            clientS?.UnloadData();
            clientS = null;

            serverS?.UnloadData();
            serverS = null;
        }
    }
}
