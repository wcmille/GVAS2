using Sandbox.ModAPI;
using System;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;

namespace GVA.NPCControl.LimitedProduction
{
    public class LimitedProduction<T> : MyGameLogicComponent where T:IMyFunctionalBlock
    {
        protected T productionBlock;
        private bool isServer;

        //Center is the Pirate Marches
        //protected readonly double dist2 = 3000000.0 * 3000000.0;

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            base.Init(objectBuilder);

            //TODO

            productionBlock = (T)Entity;
            if (productionBlock != null)
            {
                NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
                NeedsUpdate |= MyEntityUpdateEnum.EACH_10TH_FRAME;
            }
        }

        public override void UpdateOnceBeforeFrame()
        {
            base.UpdateOnceBeforeFrame();

            isServer = MyAPIGateway.Multiplayer.IsServer;

            if (isServer)
            {
                productionBlock.IsWorkingChanged += WorkingStateChange;
            }
        }

        public virtual bool Illegal()
        {
            return SharedConstants.NpcSphere.Contains(productionBlock.GetPosition()) != ContainmentType.Contains;
        }

        public override void UpdateBeforeSimulation10()
        {
            base.UpdateBeforeSimulation10();

            try
            {
                if (isServer)
                {
                    if (productionBlock.Enabled)
                    {
                        if (Illegal())
                        {
                            productionBlock.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                MyLog.Default.WriteLineAndConsole($"GVAS2 Failed: {exc}");
            }
        }

        private void WorkingStateChange(IMyCubeBlock block)
        {
            if (!productionBlock.Enabled)
            {
                if (Illegal())
                {
                    productionBlock.Enabled = false;
                }
            }
        }

        public override void Close()
        {
            if (Entity == null)
                return;
        }

        public override void OnRemovedFromScene()
        {

            base.OnRemovedFromScene();

            if (Entity == null || Entity.MarkedForClose)
            {
                return;
            }

            var Block = (T)Entity;

            if (Block == null) return;

            try
            {
                if (isServer)
                {
                    productionBlock.IsWorkingChanged -= WorkingStateChange;
                }

            }
            catch (Exception exc)
            {

                MyLog.Default.WriteLineAndConsole($"GVAS2 Failed to deregister event: {exc}");
                return;
            }
            //Unregister any handlers here
        }
    }
}
