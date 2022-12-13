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

        [ProtoMember(2)]
        public string ColorFaction;

        [ProtoMember(3)]
        public string PlayerFactionTag;
        public CommandPacket() { } // Empty constructor required for deserialization

        public CommandPacket(string pcFaction, string colorFaction, string command)
        {
            Command = command;
            PlayerFactionTag = pcFaction;
            ColorFaction = colorFaction;
        }

        public override void Execute(IWorld world)
        {
            var msg = $"PacketSimpleExample received: {ColorFaction} {PlayerFactionTag} {Command}";
            MyLog.Default.WriteLineAndConsole(msg);
            MyAPIGateway.Utilities.ShowNotification(msg);

            //Determine color.
            Accounting acct = world.GetAccountByColor(ColorFaction);
            acct.Read();

            if (acct.ColorFaction != "")
            {
                //Determine command.
                if (Command == SharedConstants.CivilianStr)
                {
                    if (acct.BuyCivilian()) world.Write(acct);
                }
                else if (Command == SharedConstants.MilitaryStr)
                {
                    if (acct.BuyMilitary()) world.Write(acct);
                }
                else if (Command == SharedConstants.CreditsStr)
                {
                    var owningFaction = MyAPIGateway.Session.Factions.TryGetFactionByTag(PlayerFactionTag);
                    long balance;
                    owningFaction.TryGetBalanceInfo(out balance);
                    if (balance >= SharedConstants.tokenPrice)
                    {
                        owningFaction.RequestChangeBalance(-SharedConstants.tokenPrice);
                        acct.AddUnspent();
                        world.Write(acct);
                    }
                }
            }
        }
    }
}