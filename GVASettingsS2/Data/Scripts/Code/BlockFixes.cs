using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage.Game.Components;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Sandbox.Game.AI.Pathfinding.Obsolete;
using Sandbox.Game.Entities;
using VRage.Game;
using VRage.ObjectBuilders;
using VRage.Game.ObjectBuilders.ComponentSystem;
using VRage.Utils;
using VRageMath;
using System.Security;


// Code is based on Gauge's Balanced Deformation code, but heavily modified for more control. 
namespace GVA.Settings
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class BlockFixes : MySessionComponentBase
    {
        public override void LoadData()
        {
            foreach (var blockDef in MyDefinitionManager.Static.GetDefinitionsOfType<MyCubeBlockDefinition>())
            {
                //Make all 5x5 XL blocks have light edge type, and no deformation, and increase weld time
                if (blockDef.CubeSize == MyCubeSize.Large && blockDef.Id.SubtypeName.Contains("XL_") && blockDef.BlockTopology == MyBlockTopology.TriangleMesh)
                {
                    blockDef.GeneralDamageMultiplier = 1.0f;
                    blockDef.UsesDeformation = false;
                    blockDef.DeformationRatio = 0.45f; //this seems to be a sweet spot between completely immune to collision, and popping with more than a light bump.
                    blockDef.EdgeType = "Light";
                    blockDef.IntegrityPointsPerSec = 2500;
                }
            }
        }
    }
}