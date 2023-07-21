using Sandbox.ModAPI;
using System;

namespace GVA.NPCControl.Server
{
    public class PirateAccount : IServerAccount, IAntagonist
    {
        readonly Random r;
        public PirateAccount(int seed = 0)
        {
            if (seed != 0) { r = new Random(seed); }
            else r = new Random();
            OwningNPCTag = SharedConstants.BlackFactionTag;
            ColorFaction = SharedConstants.BlackFactionColor;

            int mil;
            double uu;

            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.MilitaryStr}", out mil);
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.CreditsStr}", out uu);

            Military = mil;
            UnspentUnits = uu;
        }
        public double UnspentUnits { get; private set; }

        public string ColorFaction { get; private set; }

        public string OwningNPCTag { get; private set; }
        public string OwningPCTag { get { return null; } }

        public int Military { get; private set; }

        public void AddUnspent(/*PC Faction*/)
        {
            UnspentUnits += 1.0;
            //For each person in faction:
            //  Rep = (500 + rep) / 2
        }

        public bool RemoveUnspent()
        {
            return false;
        }

        public void TimePeriod(IAntagonist pirates)
        {
            Military += (int)Math.Truncate(UnspentUnits);
            UnspentUnits -= Math.Truncate(UnspentUnits);
            if (Military < 25)
            {
                Military += 1;
            }
            //Decay everyone's rep.
            //For each player in the roster...
            //If rep > -1000
            //rep -= 25
        }

        public void Read()
        {
            int mil;
            double uu;

            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.CreditsStr}", out uu);
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.MilitaryStr}", out mil);
            UnspentUnits = uu;
            Military = mil;
        }

        public void Write()
        {
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.CreditsStr}", UnspentUnits);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.MilitaryStr}", Military);
        }

        public string Log()
        {
            return "--";
        }

        public bool Fight()
        {
            int result = r.Next(4);
            if (result <= 1) return true; //opponent lost.
            if (result == 3) Military--;
            return false;
        }
    }
}
