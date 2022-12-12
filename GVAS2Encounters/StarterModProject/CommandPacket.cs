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
            string factionColor = "Blue";
            Accounting acct = world.GetAccountDetails(factionColor);
            string command = SharedConstants.CreditsStr;

            if (acct.Faction != "")
            {
                //Determine command.
                if (command == SharedConstants.CivilianStr)
                {
                    acct.BuyCivilian();
                }
                else if (command == SharedConstants.CreditsStr)
                {
                    string tag = "BMC";
                    var owningFaction = MyAPIGateway.Session.Factions.TryGetFactionByTag(tag);
                    long balance;
                    owningFaction.TryGetBalanceInfo(out balance);
                    if (balance >= SharedConstants.tokenPrice)
                    {
                        owningFaction.RequestChangeBalance(-SharedConstants.tokenPrice);
                        acct.AddUnspent();
                    }
                }
            }
        }

        public override void Execute()
        {
            //ERROR:
        }
    }
}