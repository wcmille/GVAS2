using Digi.Example_NetworkProtobuf;
using Sandbox.Game;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Game.ModAPI;

namespace GVA.NPCControl
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class Session : MySessionComponentBase
    {
        static readonly string Civilian = "Civilian";
        static readonly string Military = "Military";
        static readonly string Credits = "Credits";
        const ushort modLast4 = 7481;
        readonly ServerCommands serverCommands;

        public Networking Networking;

        public Session()
        {
            serverCommands = new ServerCommands();
            Networking = new Networking(modLast4, serverCommands);
        }

        public override void BeforeStart()
        {
            MyAPIGateway.Utilities.MessageEnteredSender += MessageEntered;
            Networking.Register();
            Time();
        }

        private void MessageEntered(ulong sender, string messageText, ref bool sendToOthers)
        {
            if (messageText == "credits")
            {
                IMyFaction faction = MyAPIGateway.Session.Factions.TryGetFactionByTag("FOO");
                if (faction != null)
                {
                    faction.RequestChangeBalance(10000000);
                    MyVisualScriptLogicProvider.SendChatMessage("Acknowledged.");
                }
            }
            else if (messageText == "blue")
            {
                int blueMilitary, blueCivilian, blueProduction;
                MyAPIGateway.Utilities.GetVariable("BlueMilitary", out blueMilitary);
                MyAPIGateway.Utilities.GetVariable("BlueCivilian", out blueCivilian);
                MyAPIGateway.Utilities.GetVariable("BlueProduction", out blueProduction);
                MyVisualScriptLogicProvider.SendChatMessage($"Blue Military Units: {blueMilitary}\nBlue Civilian: {blueCivilian}\nBlue Production: {blueProduction}");
            }
            else if (messageText == "bluemilplus5" || messageText == "blueplus5")
            {
                IncrementUnits("BlueMilitary");
            }
            else if (messageText == "bluecivplus5")
            {
                const string typeStr = "BlueCivilian";
                IncrementUnits(typeStr);
            }
            else if (messageText == "blueprodplus5")
            {
                IncrementUnits("BlueProduction");
            }
        }

        private void Time()
        {
            int civ, mil;
            double uu;
            string faction = "Blue";
            Read(faction, out civ, out mil, out uu);

            Accounting acct = new Accounting(faction, civ, mil, uu);
            acct.TimePeriod();

            Write(acct);
            WriteToClient(acct);
        }

        private static void Read(string faction, out int civ, out int mil, out double uu)
        {
            MyAPIGateway.Utilities.GetVariable($"{faction}{Civilian}", out civ);
            MyAPIGateway.Utilities.GetVariable($"{faction}{Military}", out mil);
            MyAPIGateway.Utilities.GetVariable($"{faction}{Credits}", out uu);
        }

        public static void Write(Accounting acct)
        {
            MyAPIGateway.Utilities.SetVariable($"{acct.Faction}{Civilian}", acct.Civilian);
            MyAPIGateway.Utilities.SetVariable($"{acct.Faction}{Military}", acct.Military);
            MyAPIGateway.Utilities.SetVariable($"{acct.Faction}{Credits}", acct.UnspentUnits);
        }

        private void WriteToClient(Accounting acct)
        {
            FactionValuesPacket packet = new FactionValuesPacket(acct.Faction, acct.Civilian, acct.Military, acct.UnspentUnits);
            Networking.RelayToClients(packet);
        }

        private static void IncrementUnits(string typeStr)
        {
            int value;
            MyAPIGateway.Utilities.GetVariable(typeStr, out value);
            value += 5;
            MyAPIGateway.Utilities.SetVariable(typeStr, value);
            MyVisualScriptLogicProvider.SendChatMessage($"{typeStr} Units: {value}");
        }

        protected override void UnloadData()
        {
            MyAPIGateway.Utilities.MessageEnteredSender -= MessageEntered;
            Networking?.Unregister();
            Networking = null;
        }
    }
}
