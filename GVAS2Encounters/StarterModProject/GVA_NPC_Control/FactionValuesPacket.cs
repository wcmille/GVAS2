using Digi.Example_NetworkProtobuf;
using ProtoBuf;
using Sandbox.ModAPI;
using VRage.Utils;

namespace GVA.NPCControl
{
    // An example packet extending another packet.
    // Note that it must be ProtoIncluded in PacketBase for it to work.
    [ProtoContract]
    public class FactionValuesPacket : PacketBase
    {
        // tag numbers in this class won't collide with tag numbers from the base class
        [ProtoMember(1)]
        public int Civilian;

        [ProtoMember(2)]
        public int Military;

        [ProtoMember(3)]
        public double Unspent;

        [ProtoMember(4)]
        public string Faction;

        public FactionValuesPacket() { } // Empty constructor required for deserialization

        public FactionValuesPacket(Accounting acct)
        {
            Civilian = acct.Civilian;
            Military = acct.Military;
            Unspent = acct.UnspentUnits;
            Faction = acct.Faction;
        }

        public FactionValuesPacket(string faction, int civ, int mil, double unspent)
        {
            Civilian = civ;
            Military = mil;
            Unspent = unspent;
            Faction = faction;
        }

        public override bool Received(IServerCommands server)
        {
            var msg = $"PacketSimpleExample received: Civilian={Civilian}; Military={Military}; Unspent={Unspent}";
            MyLog.Default.WriteLineAndConsole(msg);
            MyAPIGateway.Utilities.ShowNotification(msg);

            var acct = new Accounting(Faction, Civilian, Military, Unspent);
            GVA.NPCControl.Session.Write(acct);

            return true; // relay packet to other clients (only works if server receives it)
        }
    }
}