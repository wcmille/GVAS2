using ObjectBuilders.SafeZone;
using SpaceEngineers.Game.ModAPI;
using VRage.Game.Components;
using VRageMath;

namespace GVA.NPCControl.LimitedProduction
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_SafeZoneBlock), false)]
    public class LimitedProduction_SafeZoneBlock : LimitedProduction<IMySafeZoneBlock>
    {
        BoundingSphereD blackZone = new BoundingSphereD(SharedConstants.WorldCenter, 400000.0);
        BoundingSphereD blueZone = new BoundingSphereD(new Vector3D(97825.9, 79558.68, 5725255.17), 50000.0);
        BoundingSphereD redZone = new BoundingSphereD(new Vector3D(124468.67, 191328.8, 5742379.16), 50000.0);
        BoundingSphereD whiteZone = new BoundingSphereD(new Vector3D(-1854042, 131072, 2421365), 200000.0);
        BoundingSphereD greenZone = new BoundingSphereD(new Vector3D(111869, 136079, 5673610), 35000.0);
        public override bool Illegal()
        {
            Vector3D pos = productionBlock.GetPosition();
            return !(blueZone.Contains(pos) != ContainmentType.Contains && redZone.Contains(pos) != ContainmentType.Contains &&
                blackZone.Contains(pos) != ContainmentType.Contains && whiteZone.Contains(pos) != ContainmentType.Contains &&
                greenZone.Contains(pos) != ContainmentType.Contains);
        }
    }
}
