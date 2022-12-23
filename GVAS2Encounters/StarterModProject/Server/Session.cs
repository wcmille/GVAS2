using Digi.Example_NetworkProtobuf;
using Sandbox.ModAPI;
using System;
using System.Linq;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;

namespace GVA.NPCControl.Server
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class Session : MySessionComponentBase, IPacketReceiver
    {
        private Server server = null;
        private ServerWorld world = null;
        MESApi mes = null;

        //delegate bool CustomMESCondition(string spawnGroupSubId, string SpawnConditionProfile, string typeOfSpawn, Vector3D location);
        System.Func<string, string, string, Vector3D, bool> bc5, bc10, bc20, bc40;
        System.Func<string, string, string, Vector3D, bool> rc5, rc10, rc20, rc40;

        public override void LoadData()
        {
            base.LoadData();
            if (MyAPIGateway.Multiplayer.IsServer)
            {
                mes = new MESApi();
            }
        }

        public override void BeforeStart()
        {
            base.BeforeStart();
            if (MyAPIGateway.Multiplayer.IsServer)
            {
                server = new Server(Networking.ModLast4, this);
                world = new ServerWorld(server);
                world.Time();

                //MyLog.Default.WriteLine($"Blue Registration {mes.MESApiReady}");
                bc5 = (a, b, c, d) => CountersMoreThan(SharedConstants.BlueFactionColor, SharedConstants.CivilianStr, 5);
                bc10 = (a, b, c, d) => CountersMoreThan(SharedConstants.BlueFactionColor, SharedConstants.CivilianStr, 10);
                bc20 = (a, b, c, d) => CountersMoreThan(SharedConstants.BlueFactionColor, SharedConstants.CivilianStr, 20);
                bc40 = (a, b, c, d) => CountersMoreThan(SharedConstants.BlueFactionColor, SharedConstants.CivilianStr, 40);
                mes.RegisterCustomSpawnCondition(true, "BlueCivMoreThan5", bc5);
                mes.RegisterCustomSpawnCondition(true, "BlueCivMoreThan10", bc10);
                mes.RegisterCustomSpawnCondition(true, "BlueCivMoreThan20", bc20);
                mes.RegisterCustomSpawnCondition(true, "BlueCivMoreThan40", bc40);

                rc5 = (a, b, c, d) => CountersMoreThan(SharedConstants.RedFactionColor, SharedConstants.CivilianStr, 5);
                rc10 = (a, b, c, d) => CountersMoreThan(SharedConstants.RedFactionColor, SharedConstants.CivilianStr, 10);
                rc20 = (a, b, c, d) => CountersMoreThan(SharedConstants.RedFactionColor, SharedConstants.CivilianStr, 20);
                rc40 = (a, b, c, d) => CountersMoreThan(SharedConstants.RedFactionColor, SharedConstants.CivilianStr, 40);
                mes.RegisterCustomSpawnCondition(true, "RedCivMoreThan5", rc5);
                mes.RegisterCustomSpawnCondition(true, "RedCivMoreThan10", rc10);
                mes.RegisterCustomSpawnCondition(true, "RedCivMoreThan20", rc20);
                mes.RegisterCustomSpawnCondition(true, "RedCivMoreThan40", rc40);

                mes.RegisterSuccessfulSpawnAction(LogSpawn, true);
            }
        }

        private void LogSpawn(IMyCubeGrid grid)
        {
            try
            {
                var owner = grid.BigOwners.FirstOrDefault();
                var faction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(owner);
                var acct = world.GetAccountByNPCOwner(faction.Tag);
                var log = world.FetchLogs(acct);
                if (log != null)
                {
                    log.Log(grid);
                }
            }
            catch (Exception ex)
            {
                MyLog.Default.WriteLineAndConsole($"GVA_NPC_Control: ERROR - {ex.Message}");
            }
        }

        private bool CountersMoreThan(string color, string counterType, int limit)
        {
            int civ;
            MyAPIGateway.Utilities.GetVariable($"{color}{counterType}", out civ);
            return civ >= limit;
        }

        protected override void UnloadData()
        {
            if (MyAPIGateway.Multiplayer.IsServer)
            {
                mes.RegisterCustomSpawnCondition(false, "BlueCivMoreThan5", bc5);
                mes.RegisterCustomSpawnCondition(false, "BlueCivMoreThan10", bc10);
                mes.RegisterCustomSpawnCondition(false, "BlueCivMoreThan20", bc20);
                mes.RegisterCustomSpawnCondition(false, "BlueCivMoreThan40", bc40);

                mes.RegisterCustomSpawnCondition(false, "RedCivMoreThan5", rc5);
                mes.RegisterCustomSpawnCondition(false, "RedCivMoreThan10", rc10);
                mes.RegisterCustomSpawnCondition(false, "RedCivMoreThan20", rc20);
                mes.RegisterCustomSpawnCondition(false, "RedCivMoreThan40", rc40);

                mes.RegisterSuccessfulSpawnAction(LogSpawn, false);
            }

            if (server != null) server.Unload();
            server = null;
            world = null;
        }

        public void Received(PacketBase packet)
        {
            packet.Execute(world);
        }
    }
}
