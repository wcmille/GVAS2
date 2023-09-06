using Digi.Example_NetworkProtobuf;
using Sandbox.ModAPI;

namespace GVA.NPCControl.Server
{
    public class Server
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
            FactionValuesPacket packet = new FactionValuesPacket(faction?.FactionId ?? 0, acct.ColorFaction, acct.Civilian, acct.Military, acct.UnspentUnits);
            networking.RelayToClients(packet);
        }
    }
}
