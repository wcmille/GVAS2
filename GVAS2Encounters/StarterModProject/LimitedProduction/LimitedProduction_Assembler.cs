using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;
using VRage.Game.Components;

namespace GVA.NPCControl.LimitedProduction
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_Refinery), false)]
    public class LimitedProduction_Refinery : LimitedProduction<IMyRefinery>
    {
    }
}
