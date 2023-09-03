using ObjectBuilders.SafeZone;
using SpaceEngineers.Game.ModAPI;
using VRage.Game.Components;
using VRageMath;

namespace GVA.NPCControl.LimitedProduction
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_SafeZoneBlock), false)]
    public class LimitedProduction_SafeZoneBlock : LimitedProduction<IMySafeZoneBlock>
    {
        public override bool Illegal()
        {
            return SharedConstants.NpcSphere.Contains(productionBlock.GetPosition()) == ContainmentType.Contains;
        }
    }
}
