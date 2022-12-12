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

namespace GVA.NPCControl.Client
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_RadioAntenna), false, "LargeBlockRadioAntennaDish")]
    public class SatDishLogic : MyGameLogicComponent
    {
        private static bool controlsInit;
        private static IMyFaction blueFaction;
        private static IMyFaction pirateFaction;
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
                if (!MyAPIGateway.Utilities.IsDedicated)
                {
                    MyLog.Default.WriteLine("SATDISH: UpdateOnceStart - !IsDedicated");
                    blueFaction = MyAPIGateway.Session.Factions.TryGetFactionByTag(SharedConstants.BlueFactionTag);
                    pirateFaction = MyAPIGateway.Session.Factions.TryGetFactionByTag(SharedConstants.BlackFactionTag);
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
                }
                else
                {
                    MyLog.Default.WriteLine("SATDISH: UpdateOnceStart - IsDedicated");
                }
                controlsInit = true;
                MyLog.Default.WriteLine("SATDISH: UpdateOnceFinish");
            }
        }

        #region Private Methods

        private void Dish_AppendingCustomInfo(IMyTerminalBlock block, System.Text.StringBuilder builder)
        {
            var supportedfaction = SupportedFaction();
            if (supportedfaction == null || supportedfaction.Tag == SharedConstants.BlackFactionTag)
            {
                MyLog.Default.WriteLine("SATDISH: No Faction Found.");
                supportedfaction = MyAPIGateway.Session.Factions.TryGetFactionByTag(SharedConstants.BlackFactionTag);
                builder.AppendLine($"Supporting: {supportedfaction.Name}");
            }
            else if (supportedfaction != null && supportedfaction.Tag != SharedConstants.BlackFactionTag && supportedfaction.Tag == block.GetOwnerFactionTag())
            {
                builder.AppendLine($"Supporting: {blueFaction.Name}");
                int military, civilian;
                double credits;
                MyAPIGateway.Utilities.GetVariable($"{SharedConstants.BlueFactionColor}{SharedConstants.MilitaryStr}", out military);
                MyAPIGateway.Utilities.GetVariable($"{SharedConstants.BlueFactionColor}{SharedConstants.CivilianStr}", out civilian);
                MyAPIGateway.Utilities.GetVariable($"{SharedConstants.BlueFactionColor}{SharedConstants.CreditsStr}", out credits);

                builder.AppendLine($"{military} Military Units");
                builder.AppendLine($"{civilian} Civilian Units");

                builder.AppendLine();
                builder.AppendLine($"{credits:F2} Unspent NPC Units");
            }
        }

        private IMyFaction SupportedFaction()
        {
            if (jaghFactionBlock == null)
            {
                return MyAPIGateway.Session.Factions.TryGetFactionByTag(SharedConstants.BlackFactionTag);
            }
            else
            {
                var owner = jaghFactionBlock.BigOwners.First();
                var supportedfaction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(owner);
                return supportedfaction;
            }
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
            activate.Enabled = x => unconditional || x?.GameLogic?.GetAs<SatDishLogic>().SupportedFaction().Tag != SharedConstants.BlackFactionTag;
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
            AddButton("Commission Military", x => IncreaseNPCAndNotify(x, SharedConstants.BlueFactionColor, SharedConstants.MilitaryStr));
            AddButton("Commission Civilian", x => IncreaseNPCAndNotify(x, SharedConstants.BlueFactionColor, SharedConstants.CivilianStr));
            AddSeparator("FleetCommandSeparator");
        }

        private static void BuyCredits(IMyTerminalBlock block)
        {
            long credits;
            int units;
            var owningFaction = MyAPIGateway.Session.Factions.TryGetFactionByTag(block.GetOwnerFactionTag());
            if (owningFaction != null)
            {
                if (owningFaction.TryGetBalanceInfo(out credits) && credits >= SharedConstants.tokenPrice)
                {
                    MyAPIGateway.Utilities.GetVariable($"{SharedConstants.BlueFactionColor}{SharedConstants.CreditsStr}", out units);
                    MyAPIGateway.Utilities.SetVariable($"{SharedConstants.BlueFactionColor}{SharedConstants.CreditsStr}", units + 1);
                    UpdateInfo(block);
                    ClientSession.client.Send(new CommandPacket(owningFaction.Tag, SharedConstants.BlueFactionColor, SharedConstants.CreditsStr));
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

        private static void IncreaseNPCAndNotify(IMyTerminalBlock block, string factionColor, string shipType)
        {
            int amt;
            {
                double credits;
                MyAPIGateway.Utilities.GetVariable($"{factionColor}{SharedConstants.CreditsStr}", out credits);
                if (credits >= 1.0)
                {
                    ClientSession.client.Send(new CommandPacket(block.GetOwnerFactionTag(), factionColor, shipType));
                }
            }
            MyAPIGateway.Utilities.GetVariable($"{factionColor}{shipType}", out amt);
            MyAPIGateway.Utilities.SetVariable($"{factionColor}{shipType}", amt+1);

            double units;
            MyAPIGateway.Utilities.GetVariable($"{factionColor}{SharedConstants.CreditsStr}", out units);
            MyAPIGateway.Utilities.SetVariable($"{factionColor}{SharedConstants.CreditsStr}", units - 1);
            UpdateInfo(block);
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
