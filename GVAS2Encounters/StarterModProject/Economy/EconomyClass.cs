using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.IO;
using VRage.Game;
using VRage.Game.ModAPI;

namespace GVA.NPCControl.Economy
{
    public class EconomyClass
    {
        private readonly Dictionary<MyDefinitionId, double> MasterValues = new Dictionary<MyDefinitionId, double>();
        readonly List<IMySlimBlock> blocks = new List<IMySlimBlock>();

        public EconomyClass()
        {
            var allDefinitions = MyDefinitionManager.Static.GetAllDefinitions();
            foreach (var definition in allDefinitions)
            {
                if (definition is MyCubeBlockDefinition)
                {
                    var def = definition as MyCubeBlockDefinition;
                    var amt = 0.0;
                    foreach (var comp in def.Components)
                    {
                        amt += comp.Count * GetPhysicalItemPrice(comp.Definition);
                    }
                    MasterValues.Add(definition.Id, (int)amt);
                }
                else if (definition is MyPhysicalItemDefinition)
                {
                    GetPhysicalItemPrice(definition);
                }
            }

            using (var file = MyAPIGateway.Utilities.WriteFileInWorldStorage("TheThing.csv", typeof(EconomyClass)))
            {
                file.WriteLine("Name,Type,Cost (SC)");
                foreach (var definition in allDefinitions)
                {
                    var val = MasterValues.ContainsKey(definition.Id) ? MasterValues[definition.Id] : -99;
                    string middle = "???";
                    if (definition is MyPhysicalItemDefinition) middle = "Phys";
                    else if (definition is MyCubeBlockDefinition) middle = (definition as MyCubeBlockDefinition).CubeSize.ToString();
                    file.WriteLine($"{definition.DisplayNameText},{middle},{val}");
                }

                //WriteValue(file, "Yumi-class Frigate");
                //WriteValue(file, "[NPC] Industry Tower");
                WriteValue(file, "Farpoint Station");
            }
            //Loop through the blocks
        }

        private void WriteValue(TextWriter file, string name)
        {
            file.WriteLine($"{name},Ship,{CalculatePrefabCost(name)}");
        }

        private double GetPhysicalItemPrice(MyDefinitionId id)
        {
            if (MasterValues.ContainsKey(id))
            {
                return MasterValues[id];
            }
            else
            {
                return GetPhysicalItemPrice(MyDefinitionManager.Static.TryGetPhysicalItemDefinition(id));
            }
        }

        private double GetPhysicalItemPrice(MyDefinitionBase definition)
        {
            var def = definition as MyPhysicalItemDefinition;
            if (!MasterValues.ContainsKey(def.Id))
            {
                double result;
                if (def.MinimalPricePerUnit >= 0)
                {
                    result = def.MinimalPricePerUnit;
                }
                else
                {
                    MyBlueprintDefinitionBase blueprintDef;
                    if (MyDefinitionManager.Static.TryGetBlueprintDefinitionByResultId(def.Id, out blueprintDef))
                    {
                        var sum = 0.0;
                        foreach (var item in blueprintDef.Prerequisites)
                        {
                            sum += (double)item.Amount * GetPhysicalItemPrice(item.Id);
                        }
                        if (blueprintDef.Results.Length == 1) result = sum / (double)blueprintDef.Results[0].Amount;
                        else result= -1;
                    }
                    else //No blueprint
                    {
                        result = -1;
                    }
                }
                MasterValues.Add(def.Id, result);
                return result;
            }
            else
            {
                return MasterValues[def.Id];
            }
        }

        public double CalculatePrefabCost(string prefabId)
        {
            //TODO: Check For Existing Prefab
            var id = new MyDefinitionId(typeof(MyObjectBuilder_PrefabDefinition), prefabId);
            double price = 0;

            if (MasterValues.TryGetValue(id, out price))
                return price;

            var prefab = MyDefinitionManager.Static.GetPrefabDefinition(prefabId);

            if (prefab?.CubeGrids == null)
                return 0;

            foreach (var grid in prefab.CubeGrids)
            {
                if (grid.CubeBlocks == null)
                    continue;

                foreach (var block in grid.CubeBlocks)
                {
                    double thisPrice = 0;
                    MasterValues.TryGetValue(block.GetId(), out thisPrice);
                    price += thisPrice;
                }
            }
            if (price > 0)
            {
                MasterValues.Add(id, price);
            }
            return price;
        }

        public double CalculateRealShipCost(IMyCubeGrid grid)
        {
            blocks.Clear();
            grid.GetBlocks(blocks);
            double sum = 0.0;
            foreach (var block in blocks)
            { 
                sum+=GetBlockRegularValue(block, null, true);
            }
            return sum;
        }

        public double GetBlockRegularValue(IMySlimBlock block, Dictionary<string, int> missing, bool getInventoryValue)
        {
            double result;
            if (!MasterValues.TryGetValue(block.BlockDefinition.Id, out result))
                return result;

            if (missing != null)
            {
                missing.Clear();
                block.GetMissingComponents(missing);

                foreach (var compName in missing)
                {
                    var comp = new MyDefinitionId(typeof(MyObjectBuilder_Component), compName.Key);
                    double compValue = 0;
                    if (MasterValues.TryGetValue(comp, out compValue))
                        result -= compValue * compName.Value;
                }
            }

            if (getInventoryValue)
            {
                if (block.FatBlock == null || !block.FatBlock.HasInventory)
                    return result;

                var cubeBlock = block.FatBlock as MyCubeBlock;

                for (int i = 0; i < block.FatBlock.InventoryCount; i++)
                {
                    var inventory = cubeBlock.GetInventory(i);
                    foreach (var item in inventory.GetItems())
                    {

                        double itemValue = 0;

                        if (MasterValues.TryGetValue(item.Content.GetId(), out itemValue))
                            result += (int)Math.Floor(itemValue * (float)item.Amount);
                    }
                }
            }
            return result;
        }
    }
}
