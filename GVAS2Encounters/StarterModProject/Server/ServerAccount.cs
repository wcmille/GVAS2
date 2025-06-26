using Sandbox.ModAPI;
using System;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;

namespace GVA.NPCControl.Server
{
    public interface IServerAccount : IAccount
    {
        void TimePeriod(IAntagonist pirates = null);
    }

    public class ServerAccount : Accounting, IServerAccount
    {
        const double corruption = 0.9;
        const int maxRep = 1500;
        readonly IFactionsApi server;

        public ServerLog AccountLog { get; private set; }
        //readonly List<IMyIdentity> identities = new List<IMyIdentity>();

        public ServerAccount(string color, ServerLog myLog, IFactionsApi server) : base(color)
        {
            this.server = server;
            AccountLog = myLog;
            int incur;
            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.IncursionsStr}", out incur);
            Incursions = incur;
        }

        public ServerAccount(IMyFaction owner, IMyFaction npc, string f, int c, int m, double uu, IFactionsApi server = null) : base(owner, npc, f, c, m, uu)
        {
            this.server = server;
        }

        public int Incursions { get; private set; }

        public override bool BuyCivilian(int amt = 1)
        {
            var result = base.BuyCivilian(amt);
            if (result)
            {
                AccountLog.Log($"Bought {amt} {SharedConstants.CivilianStr}.");
            }
            return result;
        }

        public override bool BuyMilitary(int amt = 1)
        {
            var result = base.BuyMilitary(amt);
            if (result)
            {
                AccountLog.Log($"Bought {amt} {SharedConstants.MilitaryStr}.");
            }
            return result;
        }

        public override void AddUnspent(IMyFaction donor)
        {
            base.AddUnspent(donor);
            AccountLog.Log($"Added SC to NPC account.");
        }

        public override bool RemoveUnspent()
        {
            var result = base.RemoveUnspent();
            if (result) AccountLog.Log($"Withdrew SC to NPC account.");
            return result;
        }

        private string Log()
        {
            return $"Civ: {Civilian} Mil: {Military} Unspent: {UnspentUnits:F2}";
        }

        private void ResolveReputation(int boostCiv, int boostMil)
        {
            const double friction = 0.95;
            const int civMult = 2;
            if (OwningPCFaction != null)
            {
                int minRep = maxRep;
                foreach (var player in OwningPCFaction.Members.Keys)
                {
                    //If you are on bad terms with Silverbranch, pay debt.
                    //Write in log.
                    //If you are on bad terms with color account, pay debt.
                    //Write in log.
                    var rep = MyAPIGateway.Session.Factions.GetReputationBetweenPlayerAndFaction(player, npcOwner.FactionId);
                    //AccountLog.Log($"{MyAPIGateway.Session.}");
                    //MyLog.Default.WriteLineAndConsole($"{SharedConstants.ModName}: REZ - {player} {rep} {minRep}");
                    minRep = Math.Min(rep, minRep);
                }

                //Pay Debts
                { 
                }

                minRep = (int)((minRep - SharedConstants.AlliedRep) * friction + SharedConstants.AlliedRep);

                minRep = Math.Min(civMult * (Civilian + boostCiv) + minRep, maxRep);
                foreach (var player in OwningPCFaction.Members.Keys)
                {
                    var rep = MyAPIGateway.Session.Factions.GetReputationBetweenPlayerAndFaction(player, npcOwner.FactionId);
                    //MyLog.Default.WriteLineAndConsole($"{SharedConstants.ModName}: REZ2 - {player} {rep} {minRep}");
                    server.SetReputation(player, npcOwner.FactionId, Math.Max(rep, minRep));
                    //MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(player, npcOwner.FactionId, Math.Max(rep, minRep));
                }
                if (minRep >= maxRep) AccountLog?.Log($"{OwningPCFaction.Tag} has won.");
            }
        }

        private void ResolveEconomy(int boostCiv, int boostMil)
        {
            double netIncome = CalcNetIncome(boostCiv, boostMil);
            UnspentUnits += netIncome;
            if (UnspentUnits < 0.0)
            {
                int balance = (int)Math.Ceiling(-UnspentUnits);
                if (Military >= balance)
                {
                    Military -= balance;
                    UnspentUnits = 0.0;
                }
                else
                {
                    Military = 0;
                    Civilian--;
                    UnspentUnits = 1.0;
                }

            }
            if (UnspentUnits > 50.0)
            {
                UnspentUnits *= corruption;
            }
            if (OwningPCTag == "SPRT")
            {
                Military /= 2;
                Civilian /= 2;
                UnspentUnits /= 2;
            }
        }

        private void ResolveIncursions(IAntagonist pirates)
        {
            //Resolve Incursions from the last period.
            for (int i = 0; i < Incursions; ++i)
            {
                if (pirates.Fight())
                {
                    Civilian--;
                    AccountLog.Log("Lost civilian building to pirates.");
                }
                else
                {
                    AccountLog.Log("Defended area against pirates.");
                }
            }
            //Resolve Incursions from the new period.
            {
                Incursions = (pirates.Military - Military) / 3;
                Incursions = Math.Min(Civilian / 2, Incursions);
                Incursions = Math.Max(0, Incursions);
            }
        }

        public void TimePeriod(IAntagonist pirates = null)
        {
            CostDeterminer cd = new CostDeterminer();

            var sphere = SharedConstants.NpcSphere;
            var ents = MyAPIGateway.Entities?.GetTopMostEntitiesInSphere(ref sphere);

            int boostC = 0, boostM = 0;

            if (ents != null)
            {
                foreach (var ent in ents)
                {
                    int C = 0, M = 0;
                    if (ent is IMyCubeGrid)
                    {
                        cd.DetermineCost(ent as IMyCubeGrid, ref C, ref M, this.OwningNPCTag);
                    }
                    boostC += C;
                    boostM += M;
                }
            }

            if (pirates != null)
            {
                ResolveIncursions(pirates);
            }
            else
            {
                MyLog.Default?.WriteLine($"{SharedConstants.ModName}: Error Null Pirates");
            }
            ResolveReputation(boostC, boostM);
            ResolveEconomy(boostC, boostM);
            AccountLog?.Log(Log());
        }

        public override void Read()
        {
            int incur;
            base.Read();
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.IncursionsStr}", out incur);
            Incursions = incur;
        }

        public override void Write()
        {
            base.Write();
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.IncursionsStr}", Incursions);
        }

        internal void LogSpawn2(IMyCubeGrid grid)
        {
            AccountLog.Log(grid, Color.Green, "Site ");
        }
        internal void LogSpawn(IMyCubeGrid grid)
        {
            AccountLog.Log(grid, Color.Red, "Contact ");
        }
        internal void Mayday(IMyCubeGrid grid)
        {
            AccountLog.Log(grid, Color.Green, "Mayday ");
        }

        //internal void NewPlayer(long playerId)
        //{
        //    if (OwningPCFaction != null)
        //    {
        //        //SharedConstants.EnemyRep
        //        server.SetReputation(playerId, npcOwner.FactionId, 422);
        //    }
        //}

        internal void PlayerJoined(long playerId, IMyFaction toFaction)
        {
            //This could be any faction.
        
            if (toFaction.FounderId == playerId)
            {
                //Faction Created
                if (OwningPCFaction != null)
                {
                    var rep = MyAPIGateway.Session.Factions.GetReputationBetweenFactions(toFaction.FactionId, OwningPCFaction.FactionId);
                    server.SetReputation(playerId, npcOwner.FactionId, rep);
                }
                //Otherwise, just let the default happen.
            }
            else
            {
                var founderRep = MyAPIGateway.Session.Factions.GetReputationBetweenPlayerAndFaction(toFaction.FounderId, npcOwner.FactionId);
                var rep = SharedConstants.EnemyRep;
                if (founderRep >= SharedConstants.AlliedRep) rep = SharedConstants.AlliedRep;
                else if (founderRep >= SharedConstants.DefaultRep) rep = SharedConstants.DefaultRep;
                server.SetReputation(playerId, npcOwner.FactionId, rep);
            }
        }

        internal void PlayerLeft(long playerId)
        {
            server.SetReputation(playerId, npcOwner.FactionId, SharedConstants.DefaultRep);
        }

        internal void FactionLeft()
        {
            //Maybe I don't need this if it throws everyone out of the faction, or causes the territory to reset.
            //PC faction = null
        }

        internal void Peace(long otherFaction)
        {
            var theOtherFaction = MyAPIGateway.Session.Factions.TryGetFactionById(otherFaction);
            var npcFactionId = MyAPIGateway.Session.Factions.TryGetFactionByTag(OwningNPCTag).FactionId;

            foreach (var identity in theOtherFaction.Members.Keys)
            {
                ulong steamId = MyAPIGateway.Players.TryGetSteamId(identity);

                if (steamId > 0)
                {
                    server.SetReputation(identity, npcFactionId, SharedConstants.DefaultRep);
                    //MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(identity, npcFactionId, SharedConstants.DefaultRep);
                }
            }
        }

        internal void War(long otherFaction)
        {
            var theOtherFaction = MyAPIGateway.Session.Factions.TryGetFactionById(otherFaction);
            var npcFactionId = MyAPIGateway.Session.Factions.TryGetFactionByTag(OwningNPCTag).FactionId;

            foreach (var identity in theOtherFaction.Members.Keys)
            {
                ulong steamId = MyAPIGateway.Players.TryGetSteamId(identity);

                if (steamId > 0)
                {
                    server.SetReputation(identity, npcFactionId, SharedConstants.EnemyRep);
                    //MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(identity, npcFactionId, SharedConstants.EnemyRep);
                }
            }
        }
    }
}
