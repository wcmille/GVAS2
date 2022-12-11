using Digi.Example_NetworkProtobuf;
using ProtoBuf;
using Sandbox.ModAPI;
using VRage.Utils;

namespace GVA.NPCControl
{
    // This packet is always sent from the Client to the server.
    // Note that it must be ProtoIncluded in PacketBase for it to work.
    [ProtoContract]
    public class CommandPacket : PacketBase
    {
        // tag numbers in this class won't collide with tag numbers from the base class
        [ProtoMember(1)]
        public string Command;

        public CommandPacket() { } // Empty constructor required for deserialization

        public CommandPacket(string command)
        {
            Command = command;
        }

        public override void Execute(IWorld world)
        {
            var msg = $"PacketSimpleExample received: {Command}";
            MyLog.Default.WriteLineAndConsole(msg);
            MyAPIGateway.Utilities.ShowNotification(msg);

            //Determine color.
            string faction = "Blue";
            Accounting acct = world.GetAccountDetails(faction);
            string command = "Civilian";

            if (acct.Faction != "")
            {
                //Determine command.
                if (command == "Civilian")
                {
                    acct.BuyCivilian();
                }
            }
        }

        public override void Execute()
        {
            //ERROR:
        }
    }
}