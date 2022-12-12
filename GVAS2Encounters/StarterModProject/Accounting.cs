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

        public Accounting(string faction)
        {
            int civ, mil;
            double uu;

            MyAPIGateway.Utilities.GetVariable($"{faction}{SharedConstants.CivilianStr}", out civ);
            MyAPIGateway.Utilities.GetVariable($"{faction}{SharedConstants.MilitaryStr}", out mil);
            MyAPIGateway.Utilities.GetVariable($"{faction}{SharedConstants.CreditsStr}", out uu);

            ColorFaction = faction;
            Civilian = civ;
            Military = mil;
            UnspentUnits = uu;
        }

        public Accounting(string f, int c, int m, double uu)
        {
            ColorFaction = f;
            Civilian = c;
            Military = m;
            UnspentUnits = uu;
        }

        public int Civilian { get; private set; }
        public int Military { get; private set; }
        public double UnspentUnits { get; private set; }
        public string ColorFaction { get; private set; }

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

        public void AddUnspent()
        {
            UnspentUnits += 1.0;
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

        public void Read()
        {
            int civ, mil;
            double uu;
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.CivilianStr}", out civ);
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.MilitaryStr}", out mil);
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.CreditsStr}", out uu);
            Civilian = civ;
            Military = mil;
            UnspentUnits = uu;
        }

        public void Write()
        {
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.CivilianStr}", Civilian);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.MilitaryStr}", Military);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.CreditsStr}", UnspentUnits);
        }
    }
}
