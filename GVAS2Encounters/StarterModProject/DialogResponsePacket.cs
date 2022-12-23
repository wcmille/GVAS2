using Digi.Example_NetworkProtobuf;
using ProtoBuf;
using Sandbox.ModAPI;
using VRage.Utils;

namespace GVA.NPCControl
{
    [ProtoContract]
    public class DialogResponsePacket : PacketBase
    {
        [ProtoMember(1)]
        public string ReportText;

        public DialogResponsePacket()
        {
        }

        public DialogResponsePacket(string reportText)
        {
            this.ReportText = reportText;
            //var msg = $"SATDISH-DPACK: {ReportText}";
            //MyLog.Default.WriteLineAndConsole(msg);
        }

        public override void Execute(IWorld world)
        {
            //var msg = $"SATDISH-DPACK: {ReportText}";
            //MyLog.Default.WriteLineAndConsole(msg);
            MyAPIGateway.Utilities.ShowMissionScreen("Faction Report", null, null, ReportText);
        }
    }
}
