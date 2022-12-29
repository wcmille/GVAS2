//Done
//Correct player is charged.
//Units cost 20M to buy.
//Pirates only let you buy.

//MES CHANGES**********************************
//Charge & Refund cleanup.
//Put in Red Faction
//Put in Green Faction
//Design Blue Faction

//MOD CHANGES**********************************
//Faction is detected based on territory ownership.
//Handle Updates at specified intervals.
//When territory lost, reset all relationships
//When territory won, ally with NPCs
//Put in Red Faction
//Put in Green Faction

//OTHER CHANGES********************************
//Fix NPC economy
//Update Modlist
//Fix Ore Map

//NPC conflict occurs:
//Pirate - Red 
//Pirate - Green
//Pirate - Blue
//Red - Green
//Red - Blue
//Green - Blue

using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using System;
using System.Text;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace GVA.NPCControl.Client
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_RadioAntenna), false, "LargeBlockRadioAntennaDish")]
    public class SatDishLogic : MyGameLogicComponent
    {
        private static bool controlsInit;

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            base.Init(objectBuilder);
            NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
        }

        public override void UpdateOnceBeforeFrame()
        {
            base.UpdateOnceBeforeFrame();
            if (!controlsInit)
            {
                if (!MyAPIGateway.Utilities.IsDedicated)
                {
                    CreateControls();
                }
                else
                {
                    MyLog.Default.WriteLine("SATDISH: UpdateOnceStart - IsDedicated");
                }
                controlsInit = true;
                MyLog.Default.WriteLine($"SATDISH: UpdateOnceFinish");
            }
            var dish = Entity as IMyRadioAntenna;
            dish.AppendingCustomInfo += Dish_AppendingCustomInfo;
        }

        public override void Close()
        {
            var dish = Entity as IMyRadioAntenna;
            dish.AppendingCustomInfo -= Dish_AppendingCustomInfo;
            base.Close();
        }

        #region Private Methods

        private void Dish_AppendingCustomInfo(IMyTerminalBlock block, System.Text.StringBuilder builder)
        {
            var factionOwningTerritory = AccountOwningTerritory(block.GetOwnerFactionTag());
            //if (factionOwningTerritory == null) MyLog.Default.WriteLine("SATDISH: SupportedFaction: Null Faction");
            //else MyLog.Default.WriteLine($"SATDISH: SupportedFaction: {factionOwningTerritory.OwningNPCTag}");
            if (factionOwningTerritory == null || factionOwningTerritory.OwningNPCTag == SharedConstants.BlackFactionTag)
            {
                //MyLog.Default.WriteLine("SATDISH: No Faction Found.");
                var supportedFaction = MyAPIGateway.Session.Factions.TryGetFactionByTag(SharedConstants.BlackFactionTag);
                builder.AppendLine($"Supporting: {supportedFaction.Name}");
            }
            else if (factionOwningTerritory != null && factionOwningTerritory.OwningNPCTag != SharedConstants.BlackFactionTag && factionOwningTerritory.OwningPCTag == block.GetOwnerFactionTag())
            {
                builder.AppendLine($"Supporting: {factionOwningTerritory.OwningNPCTag}");

                builder.AppendLine($"{factionOwningTerritory.Military} Military Units");
                builder.AppendLine($"{factionOwningTerritory.Civilian} Civilian Units");

                builder.AppendLine();
                builder.AppendLine($"{factionOwningTerritory.UnspentUnits:F2} Unspent NPC Units");
            }
        }

        private static IAccount AccountOwningTerritory(string pcFactionTag)
        {
            return Client.client.World.GetAccountByPCOwner(pcFactionTag);
        }

        private static void AddLabel(string labelText)
        {
            var label = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlLabel, IMyRadioAntenna>(labelText);
            label.Enabled = x => true;
            label.SupportsMultipleBlocks = false;
            label.Visible = x => (x?.GameLogic?.GetAs<SatDishLogic>() != null);
            label.Label = MyStringId.GetOrCompute(labelText);
            MyAPIGateway.TerminalControls.AddControl<IMyRadioAntenna>(label);
        }

        private static void AddButton(string buttonText, Action<IMyTerminalBlock> action, bool unconditional = false)
        {
            var control = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlButton, IMyRadioAntenna>("GVA.SatDishLogic.BuyCredits.Button");
            control.Enabled = x => unconditional || AccountOwningTerritory(x.GetOwnerFactionTag()) is IZoneFaction;
            control.SupportsMultipleBlocks = false;
            control.Visible = x => (x?.GameLogic?.GetAs<SatDishLogic>() != null);
            control.Title = MyStringId.GetOrCompute(buttonText);
            control.Action = action;
            MyAPIGateway.TerminalControls.AddControl<IMyRadioAntenna>(control);

            StringBuilder builder = new StringBuilder(buttonText);
            var myAction = MyAPIGateway.TerminalControls.CreateAction<IMyRadioAntenna>("GVA.SatDishLogic.BuyCredits.Action");
            myAction.Name = builder;
            myAction.ValidForGroups = false;
            myAction.Action = action;
            myAction.Enabled = x => unconditional || AccountOwningTerritory(x.GetOwnerFactionTag()) is IZoneFaction; 
            MyAPIGateway.TerminalControls.AddAction<IMyRadioAntenna>(myAction);
        }

        private static void AddSeparator(string id)
        {
            var activate = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSeparator, IMyRadioAntenna>(id);
            activate.Enabled = x => true;
            activate.SupportsMultipleBlocks = false;
            activate.Visible = x => (x?.GameLogic?.GetAs<SatDishLogic>() != null);
            MyAPIGateway.TerminalControls.AddControl<IMyRadioAntenna>(activate);
        }

        private static void CreateControls()
        {
            //Label: Supported NPC Faction
            //var color = SharedConstants.BlueFactionColor;
            AddLabel("Fleet Commands");
            AddButton("Buy NPC Unit (20M SC)", BuyCredits, true);
            AddButton("Commission Military", x => IncreaseNPCAndNotify(x, SharedConstants.MilitaryStr));
            AddButton("Commission Civilian", x => IncreaseNPCAndNotify(x, SharedConstants.CivilianStr));
            AddSeparator("FleetCommandSeparator");
            AddButton("Read Log", RequestLog);
            AddSeparator("LogGroupSeparator");
        }

        private static void RequestLog(IMyTerminalBlock block)
        {
            var pcFactionTag = block.GetOwnerFactionTag();
            var acct = AccountOwningTerritory(pcFactionTag);
            //var owningFaction = MyAPIGateway.Session.Factions.TryGetFactionByTag(pcFactionTag);
            if (acct is IZoneFaction)
            {
                Client.client.World.RequestReport(MyAPIGateway.Multiplayer.MyId, acct);
            }
        }

        private static void BuyCredits(IMyTerminalBlock block)
        {
            var pcFactionTag = block.GetOwnerFactionTag();
            var owningFaction = MyAPIGateway.Session.Factions.TryGetFactionByTag(pcFactionTag);
            if (owningFaction != null)
            {
                long credits;
                if (owningFaction.TryGetBalanceInfo(out credits) && credits >= SharedConstants.tokenPrice)
                {
                    double units;
                    var acct = AccountOwningTerritory(pcFactionTag);
                    Client.client.Send(new CommandPacket(owningFaction.Tag, acct.ColorFaction, SharedConstants.CreditsStr));

                    MyAPIGateway.Utilities.GetVariable($"{acct.ColorFaction}{SharedConstants.CreditsStr}", out units);
                    MyAPIGateway.Utilities.SetVariable($"{acct.ColorFaction}{SharedConstants.CreditsStr}", units + 1);
                    UpdateInfo(block);
                }
                else
                {
                    //Send error message (Can't afford it)
                }
            }
            else
            {
                //Send error message (Can't find owning faction)
            }
        }

        private static void IncreaseNPCAndNotify(IMyTerminalBlock block, string shipType)
        {
            var pcFactionTag = block.GetOwnerFactionTag();
            var acct = AccountOwningTerritory(pcFactionTag);
            string factionColor = acct.ColorFaction;
            int amt;
            if (acct is IZoneFaction)
            {
                double credits;
                MyAPIGateway.Utilities.GetVariable($"{factionColor}{SharedConstants.CreditsStr}", out credits);
                if (credits >= 1.0)
                {
                    Client.client.Send(new CommandPacket(block.GetOwnerFactionTag(), factionColor, shipType));

                    MyAPIGateway.Utilities.GetVariable($"{factionColor}{shipType}", out amt);
                    MyAPIGateway.Utilities.SetVariable($"{factionColor}{shipType}", amt + 1);

                    double units;
                    MyAPIGateway.Utilities.GetVariable($"{factionColor}{SharedConstants.CreditsStr}", out units);
                    MyAPIGateway.Utilities.SetVariable($"{factionColor}{SharedConstants.CreditsStr}", units - 1);
                    UpdateInfo(block);
                }
            }
        }

        private static void UpdateInfo(IMyTerminalBlock block)
        {
            block.RefreshCustomInfo();
            var remember = block.ShowInToolbarConfig;
            block.ShowInToolbarConfig = !remember;
            block.ShowInToolbarConfig = remember;
        }
        #endregion
    }
}
