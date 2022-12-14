using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using System;
using System.Collections.Generic;
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
        readonly List<IAccount> owned = new List<IAccount>();

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
            
            AccountOwningTerritory(block.GetOwnerFactionTag(), owned);
            if (owned.Count == 1)
            {
                owned[0].Display(builder);
            }
        }

        private static void AccountOwningTerritory(string pcFactionTag, List<IAccount> owned)
        {
            Client.client.World.GetAccountByPCOwner(pcFactionTag, owned);
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
            var control = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlButton, IMyRadioAntenna>($"GVA.SatDishLogic.Button.{buttonText}");
            control.Enabled = x => unconditional || OneIZone(x);
            control.SupportsMultipleBlocks = false;
            control.Visible = x => (x?.GameLogic?.GetAs<SatDishLogic>() != null);
            control.Title = MyStringId.GetOrCompute(buttonText);
            control.Action = action;
            MyAPIGateway.TerminalControls.AddControl<IMyRadioAntenna>(control);

            StringBuilder builder = new StringBuilder(buttonText);
            var myAction = MyAPIGateway.TerminalControls.CreateAction<IMyRadioAntenna>($"GVA.SatDishLogic.Action.{buttonText}");
            myAction.Name = builder;
            myAction.ValidForGroups = false;
            myAction.Action = action;
            myAction.Enabled = x => unconditional || OneIZone(x);
            MyAPIGateway.TerminalControls.AddAction<IMyRadioAntenna>(myAction);
        }

        private static bool OneIZone(IMyTerminalBlock x)
        {
            List<IAccount> owned = new List<IAccount>();
            AccountOwningTerritory(x.GetOwnerFactionTag(), owned);
            return (owned.Count == 1 && owned[0] is IZoneFaction);
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
            AddSeparator("GVA.SatDishLogic.FleetCommandSeparator");
            AddButton("Read Log", RequestLog);
            AddSeparator("GVA.SatDishLogic.LogGroupSeparator");
        }

        private static void RequestLog(IMyTerminalBlock block)
        {
            var pcFactionTag = block.GetOwnerFactionTag();
            List<IAccount> owned = new List<IAccount>();
            AccountOwningTerritory(pcFactionTag, owned);
            if (owned.Count == 1)
            {
                var acct = owned[0];
                if (acct is IZoneFaction)
                {
                    Client.client.World.RequestReport(MyAPIGateway.Multiplayer.MyId, acct);
                }
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
                    //double units;
                    List<IAccount> owned = new List<IAccount>();
                    AccountOwningTerritory(pcFactionTag, owned);
                    if (owned.Count == 1)
                    {
                        var acct = owned[0];
                        acct.AddUnspent();
                        UpdateInfo(block);
                        Client.client.Send(new CommandPacket(owningFaction.Tag, acct.ColorFaction, SharedConstants.CreditsStr));
                    }
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
            List<IAccount> owned = new List<IAccount>();
            AccountOwningTerritory(pcFactionTag, owned);
            if (owned.Count == 1)
            {
                var acct = owned[0];

                string factionColor = acct.ColorFaction;
                if (acct is IZoneFaction)
                {
                    IZoneFaction zf = acct as IZoneFaction;
                    double credits;
                    MyAPIGateway.Utilities.GetVariable($"{factionColor}{SharedConstants.CreditsStr}", out credits);
                    if (credits >= 1.0)
                    {
                        if (shipType == SharedConstants.CivilianStr) zf.BuyCivilian();
                        else if (shipType == SharedConstants.MilitaryStr) zf.BuyMilitary();
                        UpdateInfo(block);
                        Client.client.Send(new CommandPacket(block.GetOwnerFactionTag(), factionColor, shipType));
                    }
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
