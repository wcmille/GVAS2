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
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace StarterModProject
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_RadioAntenna), false, "LargeBlockRadioAntennaDish")]
    public class SatDishLogic : MyGameLogicComponent
    {
        private static bool controlsInit;
        //private IMyFaction supportedfaction;
        private static IMyFaction blueFaction;
        private static IMyFaction pirateFaction;
        private static IMyFaction greenFaction;
        private static IMyFaction redFaction;
        private static readonly HashSet<IMyEntity> names = new HashSet<IMyEntity>(1);
        private static MyCubeGrid jaghFactionBlock;

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
                MyLog.Default.WriteLine("SATDISH: UpdateOnceStart");
                blueFaction = MyAPIGateway.Session.Factions.TryGetFactionByTag("JAGH");
                pirateFaction = MyAPIGateway.Session.Factions.TryGetFactionByTag("SPRT");
                names.Clear();
                MyAPIGateway.Entities.GetEntities(names, x => x != null && x.DisplayName != null && x.DisplayName.Contains("Faction Territory Claim"));
                if (names.Count == 0) MyLog.Default.WriteLine("SATDISH: No Territory Grids Found.");
                else
                {
                    MyLog.Default.WriteLine("SATDISH: Territory Grid Found.");
                    foreach (var item in names)
                    {
                        var beaconBlock = item as MyCubeGrid;
                        var beacons = ((IMyCubeGrid)beaconBlock).GetFatBlocks<IMyBeacon>();
                        if (beacons.Count() > 1) continue;
                        var beacon = beacons.First();
                        jaghFactionBlock = (MyCubeGrid)names.FirstElement();
                    }
                }
                CreateControls();
                var dish = Entity as IMyRadioAntenna;
                dish.AppendingCustomInfo += Dish_AppendingCustomInfo;
                controlsInit = true;
                MyLog.Default.WriteLine("SATDISH: UpdateOnceFinish");
            }
        }

        private void Dish_AppendingCustomInfo(IMyTerminalBlock block, System.Text.StringBuilder builder)
        {
            var supportedfaction = SupportedFaction();
            //builder.AppendLine($"Supporting: {supportedfaction?.Name}");
            if (supportedfaction == null || supportedfaction.Tag == "SPRT")
            {
                MyLog.Default.WriteLine("SATDISH: No Faction Found.");
                supportedfaction = MyAPIGateway.Session.Factions.TryGetFactionByTag("SPRT");
                builder.AppendLine($"Supporting: {supportedfaction.Name}");
            }

            else if (supportedfaction != null && supportedfaction.Tag != "SPRT" && supportedfaction.Tag == block.GetOwnerFactionTag())
            {
                builder.AppendLine($"Supporting: {blueFaction.Name}");
                int military, civilian, production, credits;
                MyAPIGateway.Utilities.GetVariable<int>("BlueMilitary", out military);
                MyAPIGateway.Utilities.GetVariable<int>("BlueCivilian", out civilian);
                MyAPIGateway.Utilities.GetVariable<int>("BlueProduction", out production);
                MyAPIGateway.Utilities.GetVariable<int>("BlueCredits", out credits);

                builder.AppendLine($"{military} Military Units");
                builder.AppendLine($"{civilian} Civilian Units");
                builder.AppendLine($"{production} Production Units");

                builder.AppendLine();
                builder.AppendLine($"{credits} Unspent NPC Units");
                builder.AppendLine("Next profit in 10 days");
            }
        }

        private IMyFaction SupportedFaction()
        {
            var owner = jaghFactionBlock.BigOwners.First();
            var supportedfaction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(owner);
            return supportedfaction;
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
            var activate = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlButton, IMyRadioAntenna>(buttonText);
            activate.Enabled = x => unconditional || x?.GameLogic?.GetAs<SatDishLogic>().SupportedFaction().Tag != "SPRT";
            activate.SupportsMultipleBlocks = false;
            activate.Visible = x => (x?.GameLogic?.GetAs<SatDishLogic>() != null);
            activate.Title = MyStringId.GetOrCompute(buttonText);
            activate.Action = action;
            MyAPIGateway.TerminalControls.AddControl<IMyRadioAntenna>(activate);
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
            AddLabel("Fleet Commands");
            AddButton("Buy NPC Unit (20M SC)", BuyCredits, true);
            AddButton("Commission Military", x=>IncreaseNPC(x, "Blue","Military"));
            AddButton("Commission Civilian", x=>IncreaseNPC(x, "Blue", "Civilian"));
            AddButton("Commission Production", x => IncreaseNPC(x, "Blue", "Production"));
            AddSeparator("FleetCommandSeparator");
            AddLabel("Fleet Reports");
            AddButton("Read Log", DishActivate);
            AddButton("Clear Log", DishActivate);
            AddSeparator("ReportingSeparator");
        }

        private static void BuyCredits(IMyTerminalBlock block)
        {
            long credits;
            int units;
            const long tokenPrice = 20000000;
            var owningFaction = MyAPIGateway.Session.Factions.TryGetFactionByTag(block.GetOwnerFactionTag());
            if (owningFaction != null)
            {
                if (owningFaction.TryGetBalanceInfo(out credits) && credits >= tokenPrice)
                {
                    owningFaction.RequestChangeBalance(-tokenPrice);
                    MyAPIGateway.Utilities.GetVariable<int>("BlueCredits", out units);
                    MyAPIGateway.Utilities.SetVariable<int>($"BlueCredits", units + 1);
                    UpdateInfo(block);
                }
            }
        }

        private static void IncreaseNPC(IMyTerminalBlock block, string faction, string shipType)
        {
            int sandboxCounter;
            int credits;

            MyAPIGateway.Utilities.GetVariable<int>("BlueCredits", out credits);

            if (credits > 0)
            {
                MyAPIGateway.Utilities.GetVariable<int>($"{faction}{shipType}", out sandboxCounter);
                MyAPIGateway.Utilities.SetVariable<int>($"{faction}{shipType}", sandboxCounter + 1);
                MyAPIGateway.Utilities.SetVariable<int>($"BlueCredits", credits - 1);
                UpdateInfo(block);
            }
        }

        private static void UpdateInfo(IMyTerminalBlock block)
        {
            block.RefreshCustomInfo();
            var remember = block.ShowInToolbarConfig;
            block.ShowInToolbarConfig = !remember;
            block.ShowInToolbarConfig = remember;
        }

        private static void DishActivate(IMyTerminalBlock block)
        {
            MyAPIGateway.Utilities.ShowMissionScreen("Faction Report",null, null, "01:03:22 11/10/22 Fought 2 Cruisers\n11:03:22 11/09/22 Delivered shipment.");
        }
    }
}
