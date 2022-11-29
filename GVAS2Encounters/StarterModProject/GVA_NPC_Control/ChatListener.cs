using Sandbox.Game;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Game.ModAPI;

namespace StarterModProject
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class ChatListener : MySessionComponentBase
    {
        public override void BeforeStart()
        {
            MyAPIGateway.Utilities.MessageEnteredSender += MessageEntered;
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
        }
    }
}
