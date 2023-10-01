using Digi.Example_NetworkProtobuf;
using ProtoBuf;
using Sandbox.ModAPI;

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

        [ProtoMember(5)]
        public long Owner;

        [ProtoMember(6)]
        public string NPCOwner;

        public FactionValuesPacket() { } // Empty constructor required for deserialization

        public FactionValuesPacket(Accounting acct)
        {
            Civilian = acct.Civilian;
            Military = acct.Military;
            Unspent = acct.UnspentUnits;
            Faction = acct.ColorFaction;
            var faction = MyAPIGateway.Session.Factions.TryGetFactionByTag(acct.OwningPCTag);
            Owner = faction.FactionId;
            NPCOwner = acct.OwningNPCTag;
        }

        public FactionValuesPacket(long owner, string npc, string faction, int civ, int mil, double unspent)
        {
            Civilian = civ;
            Military = mil;
            Unspent = unspent;
            Faction = faction;
            Owner = owner;
            NPCOwner = npc;
        }

        public override void Execute(IWorld world)
        {
            //var msg = $"PacketSimpleExample received: {Owner} {NPCOwner} {Faction} Civilian={Civilian}; Military={Military}; Unspent={Unspent}";
            //MyLog.Default.WriteLineAndConsole(msg);
            //MyAPIGateway.Utilities.ShowNotification(msg);

            var owner = MyAPIGateway.Session.Factions.TryGetFactionById(Owner);
            var npc = MyAPIGateway.Session.Factions.TryGetFactionByTag(NPCOwner);
            var acct = new Accounting(owner, npc, Faction, Civilian, Military, Unspent);
            world.Write(acct);
        }
    }
}