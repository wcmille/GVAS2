using Sandbox.ModAPI;
using System;
using System.Text;
using VRage.Utils;

namespace GVA.NPCControl
{
    public class Accounting : IAccount, IZoneFaction
    {
        const double militaryCosts = 0.2;
        const double timePeriodConst = 0.333333;
        const double pirateFactor = 0.006667;
        const double corruption = 0.9;

        public Accounting(string color)
        {
            int civ, mil, incur;
            double uu;
            string owningPCTag;
            string owningNPCTag;

            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.OwnerStr}", out owningPCTag);
            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.NPCStr}", out owningNPCTag);
            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.CivilianStr}", out civ);
            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.MilitaryStr}", out mil);
            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.IncursionsStr}", out incur);
            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.CreditsStr}", out uu);

            ColorFaction = color;
            Civilian = civ;
            Military = mil;
            Incursions = incur;
            UnspentUnits = uu;
            OwningPCTag = owningPCTag;
            OwningNPCTag = owningNPCTag;
        }

        public Accounting(string owner, string npc, string f, int c, int m, double uu)
        {
            ColorFaction = f;
            Civilian = c;
            Military = m;
            UnspentUnits = uu;
            OwningPCTag = owner;
            OwningNPCTag = npc;
        }

        public int Civilian { get; private set; }
        public int Military { get; private set; }
        public int Incursions { get; private set; }
        public double UnspentUnits { get; private set; }
        public string ColorFaction { get; private set; }

        public string OwningPCTag { get; private set; }
        public string OwningNPCTag { get; private set; }

        public void TimePeriod(IAntagonist pirates = null)
        {
            if (pirates != null)
            {
                ResolveIncursions(pirates);
            }
            else
            {
                MyLog.Default.WriteLine("SATDISH: Error Null Pirates");
            }
            ResolveEconomy();
        }

        private void ResolveIncursions(IAntagonist pirates)
        {
            //Resolve Incursions from the last period.
            for(int i = 0; i < Incursions; ++i)
            {
                if (pirates.Fight())
                {
                    Civilian--;
                    //TODO:Log the fight.
                }
                else
                { 
                    //TODO:Log the fight.
                }
            }
            //Resolve Incursions from the new period.
            {
                Incursions = (pirates.Military - Military) / 3;
                Incursions = Math.Min(Civilian / 2, Incursions);
                Incursions = Math.Max(0,Incursions);
            }
        }

        private void ResolveEconomy()
        {
            double netIncome = CalcNetIncome();

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

        private double CalcNetIncome()
        {
            double grossIncome = Civilian * timePeriodConst - Civilian * Civilian * pirateFactor;
            double expenses = Military * militaryCosts;
            double netIncome = grossIncome - expenses;
            return netIncome;
        }

        public void AddUnspent()
        {
            UnspentUnits += 1.0;
        }
        public bool RemoveUnspent()
        {
            if (UnspentUnits >= 1.0)
            {
                UnspentUnits -= 1.0;
                return true;
            }
            return false;
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

        public void Display(StringBuilder builder)
        {
            builder.AppendLine($"Supporting: {OwningNPCTag}");

            builder.AppendLine($"{Military} Military Units");
            builder.AppendLine($"{Civilian} Civilian Units");

            builder.AppendLine();
            builder.AppendLine($"{UnspentUnits:F2} Unspent NPC Units");

            builder.AppendLine();
            builder.AppendLine($"{CalcNetIncome():F2} Net Income");
        }

        public void Read()
        {
            int civ, mil, incur;
            double uu;
            string owningPCTag, owningNPCTag;

            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.OwnerStr}", out owningPCTag);
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.NPCStr}", out owningNPCTag);
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.CivilianStr}", out civ);
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.MilitaryStr}", out mil);
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.IncursionsStr}", out incur);
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.CreditsStr}", out uu);

            if (owningNPCTag == null)
            {
                if (ColorFaction == SharedConstants.BlueFactionColor) owningNPCTag = SharedConstants.BlueFactionTag;
                else if (ColorFaction == SharedConstants.RedFactionColor) owningNPCTag = SharedConstants.RedFactionTag;
            }

            Incursions = incur;
            Civilian = civ;
            Military = mil;
            UnspentUnits = uu;
            OwningPCTag = owningPCTag;
            OwningNPCTag = owningNPCTag;
        }

        public void Write()
        {
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.CivilianStr}", Civilian);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.MilitaryStr}", Military);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.IncursionsStr}", Incursions);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.CreditsStr}", UnspentUnits);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.OwnerStr}", OwningPCTag);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.NPCStr}", OwningNPCTag);
        }

        public string Log()
        {
            return $"Civ: { Civilian} Mil: { Military} Unspent: { UnspentUnits:F2}";
        }
    }
}
