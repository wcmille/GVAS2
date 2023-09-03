using Sandbox.ModAPI;
using System.Text;
using VRage.Game.ModAPI;

namespace GVA.NPCControl.Client
{
    public interface IClientAccount : IAccount
    {
        void Display(StringBuilder builder);
    }

    internal class ClientAccount : Accounting, IZoneFaction, IClientAccount
    {
        public ClientAccount(string color) : base(color)
        {
        }

        public void Display(StringBuilder builder)
        {
            builder.AppendLine($"Supporting: {OwningNPCTag}");

            builder.AppendLine($"{Military} Military Units");
            builder.AppendLine($"{Civilian} Civilian Units");

            builder.AppendLine();
            builder.AppendLine($"{UnspentUnits:F2} Unspent NPC Units");

            builder.AppendLine();
            builder.AppendLine($"{CalcNetIncome(0,0):F2} Net Income");
        }
    }

    internal class ClientPirateAccount : IAccount, IClientAccount
    {
        public string ColorFaction { get; private set; }
        public string OwningNPCTag { get; private set; }
        public string OwningPCTag { get { return null; } }
        public int Military { get; private set; }
        public double UnspentUnits { get; private set; }

        public ClientPirateAccount()
        {
            ColorFaction = SharedConstants.BlackFactionColor;
            OwningNPCTag = SharedConstants.BlackFactionTag;
            Read();
        }

        public void AddUnspent(IMyFaction donor)
        {
            //Really, we may not need to do anything on the client.
            UnspentUnits += 1.0;
            //For each person in faction:
            //  Rep = (500 + rep) / 2
        }

        public bool RemoveUnspent()
        {
            return false;
        }

        public void Display(StringBuilder builder)
        {
            builder.AppendLine($"Supporting: {OwningNPCTag}");
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
    }
}
