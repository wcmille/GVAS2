using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;
using VRage.Game.Components;

namespace GVA.NPCControl.LimitedProduction
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_Assembler), false)]
    public class LimitedProduction_Assembler : LimitedProduction<IMyAssembler>
    {
    }
}
