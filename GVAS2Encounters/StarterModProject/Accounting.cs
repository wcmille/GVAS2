using Sandbox.ModAPI;
using System;

namespace GVA.NPCControl
{
    public struct Accounting
    {
        const double militaryCosts = 0.2;
        const double timePeriodConst = 0.333333;
        const double pirateFactor = 0.006667;
        const double corruption = 0.9;
        static readonly string CivilianStr = "Civilian";
        static readonly string MilitaryStr = "Military";
        static readonly string CreditsStr = "Credits";

        public Accounting(string faction)
        {
            int civ, mil;
            double uu;

            MyAPIGateway.Utilities.GetVariable($"{faction}{CivilianStr}", out civ);
            MyAPIGateway.Utilities.GetVariable($"{faction}{MilitaryStr}", out mil);
            MyAPIGateway.Utilities.GetVariable($"{faction}{CreditsStr}", out uu);

            Faction = faction;
            Civilian = civ;
            Military = mil;
            UnspentUnits = uu;
        }

        public Accounting(string f, int c, int m, double uu)
        {
            Faction = f;
            Civilian = c;
            Military = m;
            UnspentUnits = uu;
        }

        public int Civilian { get; private set; }
        public int Military { get; private set; }
        public double UnspentUnits { get; private set; }
        public string Faction { get; private set; }

        public void TimePeriod()
        {
            double grossIncome = Civilian * timePeriodConst - Civilian * Civilian * pirateFactor;
            double expenses = Military * militaryCosts;
            double netIncome = grossIncome - expenses;

            UnspentUnits += netIncome;
            if (UnspentUnits < 0.0)
            {
                int balance = (int)Math.Ceiling(-UnspentUnits);
                Military -= balance;
            }
            if (UnspentUnits > 50.0)
            {
                UnspentUnits *= corruption;
            }
        }

        public bool BuyCivilian(int amt = 1)
        {
            if (UnspentUnits >= amt)
            {
                Civilian += amt;
                UnspentUnits -= amt;
                return true;
            }
            else return false;
        }

        public bool BuyMilitary(int amt = 1)
        {
            if (UnspentUnits >= amt)
            {
                Military += amt;
                UnspentUnits -= amt;
                return true;
            }
            else return false;
        }

        public void Write()
        {
            MyAPIGateway.Utilities.SetVariable($"{Faction}{CivilianStr}", Civilian);
            MyAPIGateway.Utilities.SetVariable($"{Faction}{MilitaryStr}", Military);
            MyAPIGateway.Utilities.SetVariable($"{Faction}{CreditsStr}", UnspentUnits);
        }
    }
}
