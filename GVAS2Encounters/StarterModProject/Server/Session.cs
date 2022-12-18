using Digi.Example_NetworkProtobuf;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Utils;
using VRageMath;

namespace GVA.NPCControl.Server
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class Session : MySessionComponentBase
    {
        private Server server = null;
        private ServerWorld world = null;
        MESApi mes = null;

        public override void LoadData()
        {
            base.LoadData();
            mes = new MESApi();
        }

        public override void BeforeStart()
        {
            base.BeforeStart();
            if (MyAPIGateway.Multiplayer.IsServer)
            {
                server = new Server(Networking.ModLast4);
                world = new ServerWorld(server);
                world.Time();
            }

            MyLog.Default.WriteLine($"Blue Registration {mes.MESApiReady}");
            mes.RegisterCustomSpawnCondition(true, "BlueCivMoreThan5", BlueCivMoreThan5);
            mes.RegisterCustomSpawnCondition(true, "BlueCivMoreThan10", BlueCivMoreThan10);
            mes.RegisterCustomSpawnCondition(true, "BlueCivMoreThan20", BlueCivMoreThan20);
            mes.RegisterCustomSpawnCondition(true, "BlueCivMoreThan40", BlueCivMoreThan40);
        }

        private bool BlueCivMoreThan5(string spawnGroupSubId, string SpawnConditionProfile, string typeOfSpawn, Vector3D location)
        {
            MyLog.Default.WriteLine("Blue 5");
            int civ;
            MyAPIGateway.Utilities.GetVariable($"{SharedConstants.BlueFactionColor}{SharedConstants.CivilianStr}", out civ);
            return civ >= 5;
        }

        private bool BlueCivMoreThan10(string spawnGroupSubId, string SpawnConditionProfile, string typeOfSpawn, Vector3D location)
        {
            int civ;
            MyAPIGateway.Utilities.GetVariable($"{SharedConstants.BlueFactionColor}{SharedConstants.CivilianStr}", out civ);
            MyLog.Default.WriteLine($"Blue 10 is {civ}");
            return civ >= 10;
        }

        private bool BlueCivMoreThan20(string spawnGroupSubId, string SpawnConditionProfile, string typeOfSpawn, Vector3D location)
        {
            MyLog.Default.WriteLine("Blue 20");
            int civ;
            MyAPIGateway.Utilities.GetVariable($"{SharedConstants.BlueFactionColor}{SharedConstants.CivilianStr}", out civ);
            return civ >= 20;
        }

        private bool BlueCivMoreThan40(string spawnGroupSubId, string SpawnConditionProfile, string typeOfSpawn, Vector3D location)
        {
            MyLog.Default.WriteLine("Blue 40");
            int civ;
            MyAPIGateway.Utilities.GetVariable($"{SharedConstants.BlueFactionColor}{SharedConstants.CivilianStr}", out civ);
            return civ >= 40;
        }

        protected override void UnloadData()
        {
            //mes.RegisterCustomSpawnCondition(false, "BlueCivMoreThan5", BlueCivMoreThan5);
            //mes.RegisterCustomSpawnCondition(false, "BlueCivMoreThan10", BlueCivMoreThan10);
            //mes.RegisterCustomSpawnCondition(false, "BlueCivMoreThan20", BlueCivMoreThan20);
            //mes.RegisterCustomSpawnCondition(false, "BlueCivMoreThan40", BlueCivMoreThan40);
            if (server != null) server.Unload();
        }
    }
}
