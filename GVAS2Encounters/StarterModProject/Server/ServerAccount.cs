using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
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
        public ServerLog AccountLog { get; private set; }
        readonly List<IMyIdentity> identities = new List<IMyIdentity>();

        public ServerAccount(string color, ServerLog myLog) : base(color)
        {
            AccountLog = myLog;
            int incur;
            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.IncursionsStr}", out incur);
            Incursions = incur;
        }

        public ServerAccount(IMyFaction owner, string npc, string f, int c, int m, double uu) : base(owner, npc, f, c, m, uu)
        {
        }

        public int Incursions { get; private set; }

        private string Log()
        {
            return $"Civ: {Civilian} Mil: {Military} Unspent: {UnspentUnits:F2}";
        }

        private void ResolveReputation(int boostCiv, int boostMil)
        { 
            //Get players in faction

            //For each player in faction...
                //If you are on bad terms with Silverbranch, pay debt.
                    //Write in log.
                //If you are on bad terms with color account, pay debt.
                    //Write in log.
                //Calc minimum reputation.

            //Reduce minimum reputation (multiply by 0.9)
            //rep += Calculate boost from entities.
            //rep += Calculate boost from unused civilian points.
            //new rep = minRep + rep;
            //For each player in faction...
                //rep = max(currentRep, newRep)
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
                MyLog.Default?.WriteLine("SATDISH: Error Null Pirates");
            }
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

        internal void LogSpawn(IMyCubeGrid grid)
        {
            AccountLog.Log(grid, Color.Red, "Contact ");
        }
        internal void Mayday(IMyCubeGrid grid)
        {
            AccountLog.Log(grid, Color.Green, "Mayday ");
        }

        internal void PlayerJoined(long playerId, IMyFaction toFaction)
        {
            //This could be any faction.
            //Get the relationship of the new faction's founder to the npc faction.
            //Set the faction member to that category.
            var npcFactionId = MyAPIGateway.Session.Factions.TryGetFactionByTag(OwningNPCTag).FactionId;
            var founderRep = MyAPIGateway.Session.Factions.GetReputationBetweenPlayerAndFaction(toFaction.FounderId, npcFactionId);

            var rep = SharedConstants.EnemyRep;
            if (founderRep >= SharedConstants.AlliedRep) rep = SharedConstants.AlliedRep;
            else if (founderRep >= SharedConstants.DefaultRep) rep = SharedConstants.DefaultRep;

            MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(playerId, npcFactionId, rep);
        }

        internal void PlayerLeft(long playerId)
        {
            var npcId = MyAPIGateway.Session.Factions.TryGetFactionByTag(OwningNPCTag).FactionId;
            MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(playerId, npcId, SharedConstants.DefaultRep);
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
                    MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(identity, npcFactionId, SharedConstants.DefaultRep);
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
                    MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(identity, npcFactionId, SharedConstants.EnemyRep);
                }
            }
        }
    }
}
