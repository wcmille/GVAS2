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

        /// <summary>
        /// What NPC is receiving the command?
        /// </summary>
        [ProtoMember(2)]
        public string ColorFaction;

        /// <summary>
        /// What PC Faction is issuing the command?
        /// </summary>
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
            IAccount acct = world.GetAccountByColor(ColorFaction);
            acct.Read();

            if (acct.ColorFaction != "")
            {
                //Determine command.
                if (acct is IZoneFaction)
                {
                    var ac = acct as IZoneFaction;
                    if (Command == SharedConstants.CivilianStr)
                    {
                        if (ac.BuyCivilian()) world.Write(acct);
                    }
                    else if (Command == SharedConstants.MilitaryStr)
                    {
                        if (ac.BuyMilitary()) world.Write(acct);
                    }
                }
                else if (Command == SharedConstants.CreditsStr)
                {
                    AddCredits(world, acct);
                }
            }
        }

        private void AddCredits(IWorld world, IAccount acct)
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