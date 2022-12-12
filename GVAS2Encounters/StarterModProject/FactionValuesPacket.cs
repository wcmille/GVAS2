using Digi.Example_NetworkProtobuf;
using ProtoBuf;
using Sandbox.ModAPI;
using VRage.Utils;

namespace GVA.NPCControl
{
    //This packet is always sent from the server to the client to correct a faction's values.

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
            Faction = acct.ColorFaction;
        }

        public FactionValuesPacket(string faction, int civ, int mil, double unspent)
        {
            Civilian = civ;
            Military = mil;
            Unspent = unspent;
            Faction = faction;
        }

        public override void Execute()
        {
            var msg = $"PacketSimpleExample received: Civilian={Civilian}; Military={Military}; Unspent={Unspent}";
            MyLog.Default.WriteLineAndConsole(msg);
            MyAPIGateway.Utilities.ShowNotification(msg);

            var acct = new Accounting(Faction, Civilian, Military, Unspent);
            acct.Write();
        }

        public override void Execute(IWorld world)
        {
            //ERROR:
        }
    }
}