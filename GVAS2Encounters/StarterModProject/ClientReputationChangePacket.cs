using Digi.Example_NetworkProtobuf;
using ProtoBuf;
using Sandbox.ModAPI;

namespace GVA.NPCControl
{
    [ProtoContract]
    public class ClientReputationChangePacket : PacketBase
    {
        [ProtoMember(1)]
        public long playerID;

        [ProtoMember(2)]
        public long factionID;

        [ProtoMember(3)]
        public int reputation;

        public ClientReputationChangePacket()
        {
        }

        public ClientReputationChangePacket(long playerID, long factionID, int reputation)
        {
            this.playerID = playerID;
            this.factionID = factionID;
            this.reputation = reputation;
        }

        public override void Execute(IWorld world)
        {
            MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(playerID, factionID, reputation);
        }
    }
}
