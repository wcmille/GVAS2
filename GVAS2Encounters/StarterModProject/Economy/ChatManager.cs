using Sandbox.Game;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;

namespace GVA.NPCControl.Economy
{
	public class ChatManager : IDisposable
	{
        readonly EconomyClass eco;
        const double findGridRadius = 100.0;
        public ChatManager(EconomyClass eco)
        {
            MyAPIGateway.Utilities.MessageEntered += ChatReceived;
            this.eco = eco;
        }

        private void ChatReceived(string messageText, ref bool sendToOthers)
        {
            var thisPlayer = MyAPIGateway.Session.LocalHumanPlayer;

            if (MyAPIGateway.Session.LocalHumanPlayer == null)
            {
                throw new ArgumentException($"Local Human Player is null and cannot be. Msg: {messageText}");
            }

            bool isAdmin = false;

            if (messageText.StartsWith("/Price"))
            {
                sendToOthers = false;

                if (thisPlayer.PromoteLevel == MyPromoteLevel.Admin || thisPlayer.PromoteLevel == MyPromoteLevel.Owner)
                {
                    isAdmin = true;
                }

                if (!isAdmin) MyVisualScriptLogicProvider.ShowNotification("Access Denied. Modular Encounters Systems Chat Commands Only Available To Admin Players.", 5000, "Red", thisPlayer.IdentityId);
                else
                {
                    var ships = FindGrid(thisPlayer);
                    string reportText = "Cost,Name\n";
                    foreach (var ship in ships)
                    {
                        var amt = eco.CalculateRealShipCost(ship);
                        reportText += $"{amt},{ship.CustomName}\n";
                    }
                    MyAPIGateway.Utilities.ShowMissionScreen("Grid Cost", null, null, reportText);
                    MyClipboardHelper.SetClipboard(reportText);
                }
            }
        }

        private List<IMyCubeGrid> FindGrid(IMyPlayer player)
        {
            BoundingSphereD sphere = new BoundingSphereD(player.Character.GetPosition(), findGridRadius);
            return MyAPIGateway.Entities.GetEntitiesInSphere(ref sphere).Where(x => x is IMyCubeGrid).Select(x => x as IMyCubeGrid).ToList();
        }

        public void Dispose()
        {
            MyAPIGateway.Utilities.MessageEntered -= ChatReceived;
        }
    }
}
