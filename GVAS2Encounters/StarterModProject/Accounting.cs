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
        const double militaryCosts = 0.1;
        const double timePeriodConst = 0.333333;
        const double pirateFactor = 0.006667;
        IMyFaction pcOwner;
        protected IMyFaction npcOwner;

        public Accounting(string color)
        {
            int civ, mil;
            double uu;
            string owningNPCTag;
            long owningPCFactionId;

            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.OwnerId}", out owningPCFactionId);
            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.NPCStr}", out owningNPCTag);
            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.CivilianStr}", out civ);
            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.MilitaryStr}", out mil);
            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.CreditsStr}", out uu);

            ColorFaction = color;
            Civilian = civ;
            Military = mil;
            UnspentUnits = uu;
            pcOwner = MyAPIGateway.Session.Factions.TryGetFactionById(owningPCFactionId);
            
            npcOwner = owningNPCTag == null ? null : MyAPIGateway.Session.Factions.TryGetFactionByTag(owningNPCTag);
            //OwningNPCTag = owningNPCTag;
        }

        public Accounting(IMyFaction pcOwner, IMyFaction npc, string color, int c, int m, double uu)
        {
            this.pcOwner = pcOwner;
            ColorFaction = color;
            Civilian = c;
            Military = m;
            UnspentUnits = uu;
            npcOwner = npc;
        }

        public int Civilian { get; protected set; }
        public int Military { get; protected set; }
        public double UnspentUnits { get; protected set; }
        public string ColorFaction { get; private set; }

        public string OwningPCTag
        {
            get
            {
                return pcOwner?.Tag ?? "SPRT";
            }
        }

        public string OwningNPCTag
        {
            get 
            {
                return npcOwner?.Tag;
            }
        }

        /// Return null if unowned.
        public IMyFaction OwningPCFaction
        {
            get
            {
                return pcOwner;
            }
        }

        protected double CalcNetIncome(int civ, int mil)
        {
            civ += Civilian;
            mil += Military;

            double grossIncome = civ * timePeriodConst;
            double expenses = mil * militaryCosts;
            double netIncome = grossIncome - expenses;
            return netIncome;
        }

        public virtual void AddUnspent(IMyFaction donor)
        {
            UnspentUnits += 1.0;
        }

        public virtual bool RemoveUnspent()
        {
            if (UnspentUnits >= 1.0)
            {
                UnspentUnits -= 1.0;
                return true;
            }
            return false;
        }

        public virtual bool BuyCivilian(int amt = 1)
        {
            if (UnspentUnits >= amt)
            {
                Civilian += amt;
                UnspentUnits -= amt;
                return true;
            }
            else return false;
        }

        public virtual bool BuyMilitary(int amt = 1)
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
            string owningNPCTag;
            long owningPCId;

            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.OwnerId}", out owningPCId);
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
            pcOwner = MyAPIGateway.Session.Factions.TryGetFactionById(owningPCId);
            npcOwner = MyAPIGateway.Session.Factions.TryGetFactionByTag(owningNPCTag);
        }

        public virtual void Write()
        {
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.CivilianStr}", Civilian);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.MilitaryStr}", Military);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.CreditsStr}", UnspentUnits);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.OwnerId}", pcOwner?.FactionId ?? 0);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.NPCStr}", OwningNPCTag);
        }
    }
}
