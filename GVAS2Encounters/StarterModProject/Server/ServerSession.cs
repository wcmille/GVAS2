using Digi.Example_NetworkProtobuf;
using Sandbox.ModAPI;
using System;
using System.Linq;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;

namespace GVA.NPCControl.Server
{
    public class ServerSession
    {
        private Server server = null;
        private ServerWorld world = null;
        readonly MESApi mes = null;

        //delegate bool CustomMESCondition(string spawnGroupSubId, string SpawnConditionProfile, string typeOfSpawn, Vector3D location);

        public ServerSession()
        {
            if (MyAPIGateway.Multiplayer.IsServer)
            {
                mes = new MESApi();
            }
        }
        public void UnloadData()
        {
            if (MyAPIGateway.Multiplayer.IsServer)
            {
                mes.RegisterCustomSpawnCondition(false, "BlueCivMoreThan5", null);
                mes.RegisterCustomSpawnCondition(false, "BlueCivMoreThan10", null);
                mes.RegisterCustomSpawnCondition(false, "BlueCivMoreThan20", null);
                mes.RegisterCustomSpawnCondition(false, "BlueCivMoreThan40", null);

                mes.RegisterCustomSpawnCondition(false, "RedCivMoreThan5", null);
                mes.RegisterCustomSpawnCondition(false, "RedCivMoreThan10", null);
                mes.RegisterCustomSpawnCondition(false, "RedCivMoreThan20", null);
                mes.RegisterCustomSpawnCondition(false, "RedCivMoreThan40", null);

                mes.RegisterSuccessfulSpawnAction(LogSpawn, false);
            }

            if (server != null) server.Unload();
            server = null;
            world = null;
        }

        public void BeforeStart(INetworking networking)
        {
            if (MyAPIGateway.Multiplayer.IsServer)
            {
                server = new Server(networking);
                world = new ServerWorld(server);
                world.Time();

                Func<string, string, string, Vector3D, bool> bc5, bc10, bc20, bc40;
                Func<string, string, string, Vector3D, bool> rc5, rc10, rc20, rc40;

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

        public void Received(PacketBase packet)
        {
            packet.Execute(world);
        }
    }
}
