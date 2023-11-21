using Digi.Example_NetworkProtobuf;
using Sandbox.ModAPI;

namespace GVA.NPCControl.Server
{
    public interface IFactionsApi
    {
        void SetReputation(long playerID, long factionID, int reputation);
    }

    public class Server : IFactionsApi
    {
        INetworking networking;

        public Server(INetworking networking)
        {
            this.networking = networking;
            networking.Register();
        }

        internal void Unload()
        {
            networking?.Unregister();
            networking = null;
        }

        public void WriteToClient(ulong client, ServerLog log)
        {
            var packet = new DialogResponsePacket(log.LogText);
            networking.SendToPlayer(packet, client);
        }

        public void WriteToClient(Accounting acct)
        {
            var faction = MyAPIGateway.Session.Factions.TryGetFactionByTag(acct.OwningPCTag);
            FactionValuesPacket packet = new FactionValuesPacket(acct);
            networking.RelayToClients(packet);
        }

        public void SetReputation(long playerID, long factionID, int reputation)
        {
            MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(playerID, factionID, reputation);
            var packet = new ClientReputationChangePacket(playerID, factionID, reputation);
            networking.RelayToClients(packet);
        }
    }
}
