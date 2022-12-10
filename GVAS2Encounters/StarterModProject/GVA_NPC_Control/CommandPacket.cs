using Digi.Example_NetworkProtobuf;
using ProtoBuf;
using Sandbox.ModAPI;
using VRage.Utils;

namespace GVA.NPCControl
{
    // An example packet extending another packet.
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

        public override bool Received(IServerCommands server)
        {
            var msg = $"PacketSimpleExample received: {Command}";
            MyLog.Default.WriteLineAndConsole(msg);
            MyAPIGateway.Utilities.ShowNotification(msg);

            server.Execute(Command);

            return false; // relay packet to other clients (only works if server receives it)
        }
    }
}