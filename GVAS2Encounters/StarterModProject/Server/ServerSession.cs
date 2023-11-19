using Digi.Example_NetworkProtobuf;

using Sandbox.ModAPI;
using System;
using System.Linq;
using VRage.Game.ModAPI;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;

namespace GVA.NPCControl.Server
{
    public class CostDeterminer
    {
        readonly MyIni ini = new MyIni();

        public void DetermineCost(IMyCubeGrid cg, ref int c, ref int m, string factionTag)
        {
            foreach (var b in cg.GetFatBlocks<IMyBeacon>())
            {
                if (b.GetOwnerFactionTag() == factionTag && ini.TryParse(b.CustomData))
                {
                    if (ini.ContainsSection(SharedConstants.PointCheck))
                    {
                        c = ini.Get(SharedConstants.PointCheck, SharedConstants.CivilianStr).ToInt32(0);
                        m = ini.Get(SharedConstants.PointCheck, SharedConstants.MilitaryStr).ToInt32(0);
                    }
                }
            }
        }
    }

    public class ServerSession
    {
        private Server server = null;
        private ServerWorld world = null;
        readonly MESApi mes = null;

        //delegate bool CustomMESCondition(string spawnGroupSubId, string SpawnConditionProfile, string typeOfSpawn, Vector3D location);
        readonly CostDeterminer cd = new CostDeterminer();

        public ServerSession()
        {
            if (MyAPIGateway.Multiplayer.IsServer)
            {
                mes = new MESApi();
            }
        }
        public void UnloadData()
        {
            const bool reg = false;
            if (MyAPIGateway.Multiplayer.IsServer)
            {
                mes.RegisterCustomSpawnCondition(reg, "BlueCivMoreThan2", null);
                mes.RegisterCustomSpawnCondition(reg, "BlueCivMoreThan5", null);
                mes.RegisterCustomSpawnCondition(reg, "BlueCivMoreThan10", null);
                mes.RegisterCustomSpawnCondition(reg, "BlueCivMoreThan20", null);
                mes.RegisterCustomSpawnCondition(reg, "BlueCivMoreThan40", null);
                mes.RegisterCustomSpawnCondition(reg, "BlueCivMoreThan50", null);

                mes.RegisterCustomSpawnCondition(reg, "RedCivMoreThan2", null);
                mes.RegisterCustomSpawnCondition(reg, "RedCivMoreThan5", null);
                mes.RegisterCustomSpawnCondition(reg, "RedCivMoreThan10", null);
                mes.RegisterCustomSpawnCondition(reg, "RedCivMoreThan20", null);
                mes.RegisterCustomSpawnCondition(reg, "RedCivMoreThan40", null);
                mes.RegisterCustomSpawnCondition(reg, "RedCivMoreThan50", null);

                mes.RegisterCustomSpawnCondition(reg, "BlueMilMoreThan1", null);
                mes.RegisterCustomSpawnCondition(reg, "RedMilMoreThan1", null);
                mes.RegisterCustomSpawnCondition(reg, "BlueMilMoreThan2", null);
                mes.RegisterCustomSpawnCondition(reg, "RedMilMoreThan2", null);
                mes.RegisterCustomSpawnCondition(reg, "BlueIsNormal", null);
                mes.RegisterCustomSpawnCondition(reg, "RedIsNormal", null);
                mes.RegisterCustomSpawnCondition(reg, "BlueIsStrong", null);
                mes.RegisterCustomSpawnCondition(reg, "RedIsStrong", null);

                mes.RegisterCustomSpawnCondition(reg, "BlackIsWeak", null);
                mes.RegisterCustomSpawnCondition(reg, "BlackIsNormal", null);
                mes.RegisterCustomSpawnCondition(reg, "BlackIsStrong", null);

                mes.BehaviorTriggerActivationWatcher(reg, WatchTriggers);

                mes.RegisterSuccessfulSpawnAction(LogSpawn, reg);
            }

            server?.Unload();

            server = null;
            world = null;
        }

        public void BeforeStart(INetworking networking)
        {
            const bool reg = true;

            if (MyAPIGateway.Multiplayer.IsServer)
            {
                server = new Server(networking);
                world = new ServerWorld(server);
                world.Time();

                Func<string, string, string, Vector3D, bool> bc2, bc5, bc10, bc20, bc40, bc50;
                Func<string, string, string, Vector3D, bool> rc2, rc5, rc10, rc20, rc40, rc50;

                //MyLog.Default.WriteLine($"Blue Registration {mes.MESApiReady}");
                bc2 = (a, b, c, d) => CountersMoreThan(SharedConstants.BlueFactionColor, SharedConstants.CivilianStr, 2);
                bc5 = (a, b, c, d) => CountersMoreThan(SharedConstants.BlueFactionColor, SharedConstants.CivilianStr, 5);
                bc10 = (a, b, c, d) => CountersMoreThan(SharedConstants.BlueFactionColor, SharedConstants.CivilianStr, 10);
                bc20 = (a, b, c, d) => CountersMoreThan(SharedConstants.BlueFactionColor, SharedConstants.CivilianStr, 20);
                bc40 = (a, b, c, d) => CountersMoreThan(SharedConstants.BlueFactionColor, SharedConstants.CivilianStr, 40);
                bc50 = (a, b, c, d) => CountersMoreThan(SharedConstants.BlueFactionColor, SharedConstants.CivilianStr, 50);
                mes.RegisterCustomSpawnCondition(reg, "BlueCivMoreThan2", bc2);
                mes.RegisterCustomSpawnCondition(reg, "BlueCivMoreThan5", bc5);
                mes.RegisterCustomSpawnCondition(reg, "BlueCivMoreThan10", bc10);
                mes.RegisterCustomSpawnCondition(reg, "BlueCivMoreThan20", bc20);
                mes.RegisterCustomSpawnCondition(reg, "BlueCivMoreThan40", bc40);
                mes.RegisterCustomSpawnCondition(reg, "BlueCivMoreThan50", bc50);

                rc2 = (a, b, c, d) => CountersMoreThan(SharedConstants.RedFactionColor, SharedConstants.CivilianStr, 2);
                rc5 = (a, b, c, d) => CountersMoreThan(SharedConstants.RedFactionColor, SharedConstants.CivilianStr, 5);
                rc10 = (a, b, c, d) => CountersMoreThan(SharedConstants.RedFactionColor, SharedConstants.CivilianStr, 10);
                rc20 = (a, b, c, d) => CountersMoreThan(SharedConstants.RedFactionColor, SharedConstants.CivilianStr, 20);
                rc40 = (a, b, c, d) => CountersMoreThan(SharedConstants.RedFactionColor, SharedConstants.CivilianStr, 40);
                rc50 = (a, b, c, d) => CountersMoreThan(SharedConstants.RedFactionColor, SharedConstants.CivilianStr, 50);
                mes.RegisterCustomSpawnCondition(reg, "RedCivMoreThan2", rc2);
                mes.RegisterCustomSpawnCondition(reg, "RedCivMoreThan5", rc5);
                mes.RegisterCustomSpawnCondition(reg, "RedCivMoreThan10", rc10);
                mes.RegisterCustomSpawnCondition(reg, "RedCivMoreThan20", rc20);
                mes.RegisterCustomSpawnCondition(reg, "RedCivMoreThan40", rc40);
                mes.RegisterCustomSpawnCondition(reg, "RedCivMoreThan50", rc50);

                Func<string, string, string, Vector3D, bool> bm1, bm2, rm1, rm2, bm10, rm10, bm20, rm20;
                bm1 = (a, b, c, d) => CountersMoreThan(SharedConstants.BlueFactionColor, SharedConstants.MilitaryStr, 1);
                rm1 = (a, b, c, d) => CountersMoreThan(SharedConstants.RedFactionColor, SharedConstants.MilitaryStr, 1);
                bm2 = (a, b, c, d) => CountersMoreThan(SharedConstants.BlueFactionColor, SharedConstants.MilitaryStr, 2);
                rm2 = (a, b, c, d) => CountersMoreThan(SharedConstants.RedFactionColor, SharedConstants.MilitaryStr, 2);
                bm10 = (a, b, c, d) => CountersMoreThan(SharedConstants.BlueFactionColor, SharedConstants.MilitaryStr, 10);
                rm10 = (a, b, c, d) => CountersMoreThan(SharedConstants.RedFactionColor, SharedConstants.MilitaryStr, 10);
                bm20 = (a, b, c, d) => CountersMoreThan(SharedConstants.BlueFactionColor, SharedConstants.MilitaryStr, 20);
                rm20 = (a, b, c, d) => CountersMoreThan(SharedConstants.RedFactionColor, SharedConstants.MilitaryStr, 20);
                mes.RegisterCustomSpawnCondition(reg, "BlueMilMoreThan1", bm1);
                mes.RegisterCustomSpawnCondition(reg, "RedMilMoreThan1", rm1);
                mes.RegisterCustomSpawnCondition(reg, "BlueMilMoreThan2", bm2);
                mes.RegisterCustomSpawnCondition(reg, "RedMilMoreThan2", rm2);
                mes.RegisterCustomSpawnCondition(reg, "BlueIsNormal", bm10);
                mes.RegisterCustomSpawnCondition(reg, "RedIsNormal", rm10);
                mes.RegisterCustomSpawnCondition(reg, "BlueIsStrong", bm20);
                mes.RegisterCustomSpawnCondition(reg, "RedIsStrong", rm20);


                Func<string, string, string, Vector3D, bool> biw, bin, bis;
                biw = (a, b, c, d) => CountersMoreThan(SharedConstants.BlackFactionColor, SharedConstants.MilitaryStr, 3);
                bin = (a, b, c, d) => CountersMoreThan(SharedConstants.BlackFactionColor, SharedConstants.MilitaryStr, 10);
                bis = (a, b, c, d) => CountersMoreThan(SharedConstants.BlackFactionColor, SharedConstants.MilitaryStr, 20);

                mes.RegisterCustomSpawnCondition(reg, "BlackIsWeak", biw);
                mes.RegisterCustomSpawnCondition(reg, "BlackIsNormal", bin);
                mes.RegisterCustomSpawnCondition(reg, "BlackIsStrong", bis);

                mes.BehaviorTriggerActivationWatcher(reg, WatchTriggers);

                mes.RegisterSuccessfulSpawnAction(LogSpawn, reg);
            }
        }

        private void WatchTriggers(IMyRemoteControl rc, string trigger, string action, IMyEntity target, Vector3D waypoint)
        {
            if (action == "GVA-Action-API-Despawn")
            {
                IAccount acct = GetAccountFromRC(rc);
                if (acct != null)
                {
                    int c = 0;
                    int m = 0;
                    cd.DetermineCost(rc.CubeGrid, ref c, ref m, rc.GetOwnerFactionTag());
                    Refund(acct, c, m);
                }
            }
            else if (action == "GVA-Action-API-Spawn")
            {
                IAccount acct = GetAccountFromRC(rc);
                if (acct != null)
                {
                    int c = 0;
                    int m = 0;
                    cd.DetermineCost(rc.CubeGrid, ref c, ref m, rc.GetOwnerFactionTag());
                    Pay(acct, c, m);
                }
            }
            else if (action == "GVA-Action-API-ReportContact")
            {
                var acct = GetAccountFromRC(rc);
                if (target is IMyCubeGrid)
                {
                    (acct as ServerAccount)?.LogSpawn(target as IMyCubeGrid);
                }
            }
            else if (action == "GVA-Action-API-Mayday")
            {
                var acct = GetAccountFromRC(rc);
                (acct as ServerAccount)?.Mayday(rc.CubeGrid);
            }
        }

        private static void Pay(IAccount acct, int c, int m)
        {
            //Refund costs

            int currC, currM;
            MyAPIGateway.Utilities.GetVariable($"{acct.ColorFaction}{SharedConstants.CivilianStr}", out currC);
            MyAPIGateway.Utilities.GetVariable($"{acct.ColorFaction}{SharedConstants.MilitaryStr}", out currM);

            MyAPIGateway.Utilities.SetVariable($"{acct.ColorFaction}{SharedConstants.CivilianStr}", currC - c);
            MyAPIGateway.Utilities.SetVariable($"{acct.ColorFaction}{SharedConstants.MilitaryStr}", currM - m);
        }

        private static void Refund(IAccount acct, int c, int m)
        {
            //Refund costs

            int currC, currM;
            MyAPIGateway.Utilities.GetVariable($"{acct.ColorFaction}{SharedConstants.CivilianStr}", out currC);
            MyAPIGateway.Utilities.GetVariable($"{acct.ColorFaction}{SharedConstants.MilitaryStr}", out currM);

            MyAPIGateway.Utilities.SetVariable($"{acct.ColorFaction}{SharedConstants.CivilianStr}", currC + c);
            MyAPIGateway.Utilities.SetVariable($"{acct.ColorFaction}{SharedConstants.MilitaryStr}", currM + m);
        }

        private void LogSpawn(IMyCubeGrid grid)
        {
            try
            {
                //IAccount acct = GetAccountFromGrid(grid);
                //(acct as ServerAccount)?.LogSpawn(grid);
            }
            catch (Exception ex)
            {
                MyLog.Default.WriteLineAndConsole($"{SharedConstants.ModName}: ERROR - {ex.Message}");
            }
        }

        private IAccount GetAccountFromRC(IMyRemoteControl rc)
        {
            var owner = rc.OwnerId;
            var faction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(owner);
            var acct = world.GetAccountByNPCOwner(faction.Tag);
            return acct;
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
