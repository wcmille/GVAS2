using Sandbox.ModAPI;
using VRage.Game.ModAPI;

namespace GVA.NPCControl
{
    public interface IZoneFaction
    {
        bool BuyCivilian(int amt = 1);
        bool BuyMilitary(int amt = 1);
    }

    public class Accounting : IAccount, IZoneFaction
    {
        const double militaryCosts = 0.2;
        const double timePeriodConst = 0.333333;
        const double pirateFactor = 0.006667;

        public Accounting(string color)
        {
            int civ, mil;
            double uu;
            string owningPCTag;
            string owningNPCTag;

            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.OwnerStr}", out owningPCTag);
            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.NPCStr}", out owningNPCTag);
            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.CivilianStr}", out civ);
            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.MilitaryStr}", out mil);
            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.CreditsStr}", out uu);

            ColorFaction = color;
            Civilian = civ;
            Military = mil;
            UnspentUnits = uu;
            OwningPCTag = owningPCTag;
            OwningNPCTag = owningNPCTag;
        }

        public Accounting(string owner, string npc, string color, int c, int m, double uu)
        {
            ColorFaction = color;
            Civilian = c;
            Military = m;
            UnspentUnits = uu;
            OwningPCTag = owner;
            OwningNPCTag = npc;
        }

        public int Civilian { get; protected set; }
        public int Military { get; protected set; }
        public double UnspentUnits { get; protected set; }
        public string ColorFaction { get; private set; }

        public string OwningPCTag { get; private set; }
        public string OwningNPCTag { get; private set; }

        protected double CalcNetIncome(int civ, int mil)
        {
            civ += Civilian;
            mil += Military;

            double grossIncome = civ * timePeriodConst - civ * civ * pirateFactor;
            double expenses = mil * militaryCosts;
            double netIncome = grossIncome - expenses;
            return netIncome;
        }

        public void AddUnspent(IMyFaction donor)
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

        public virtual void Read()
        {
            int civ, mil;
            double uu;
            string owningPCTag, owningNPCTag;

            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.OwnerStr}", out owningPCTag);
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.NPCStr}", out owningNPCTag);
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.CivilianStr}", out civ);
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.MilitaryStr}", out mil);
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.CreditsStr}", out uu);

            if (owningNPCTag == null)
            {
                if (ColorFaction == SharedConstants.BlueFactionColor) owningNPCTag = SharedConstants.BlueFactionTag;
                else if (ColorFaction == SharedConstants.RedFactionColor) owningNPCTag = SharedConstants.RedFactionTag;
            }

            Civilian = civ;
            Military = mil;
            UnspentUnits = uu;
            OwningPCTag = owningPCTag;
            OwningNPCTag = owningNPCTag;
        }

        public virtual void Write()
        {
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.CivilianStr}", Civilian);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.MilitaryStr}", Military);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.CreditsStr}", UnspentUnits);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.OwnerStr}", OwningPCTag);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.NPCStr}", OwningNPCTag);
        }
    }
}
