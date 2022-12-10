using System;

namespace GVA.NPCControl
{
    public struct Accounting
    {
        const double militaryCosts = 0.2;
        const double timePeriodConst = 0.333333;
        const double pirateFactor = 0.006667;
        const double corruption = 0.9;

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
    }
}
