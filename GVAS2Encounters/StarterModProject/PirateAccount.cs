using Sandbox.ModAPI;
using System.Text;

namespace GVA.NPCControl
{
    public class PirateAccount : IAccount
    {
        public PirateAccount()
        {
            OwningNPCTag = SharedConstants.BlackFactionTag;
            ColorFaction = SharedConstants.BlackFactionColor;
        }
        public double UnspentUnits { get; private set; }

        public string ColorFaction { get; private set; }

        public string OwningNPCTag { get; private set; }
        public string OwningPCTag { get { return null; } }

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

        public void TimePeriod()
        {
            //Decay everyone's rep.
                //For each player in the roster...
                //If rep > -1000
                    //rep -= 25
        }

        public void Read()
        {
            double uu;

            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.CreditsStr}", out uu);
            UnspentUnits = uu;
        }

        public void Write()
        {
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.CreditsStr}", UnspentUnits);
        }

        public void Display(StringBuilder builder)
        {
            builder.AppendLine($"Supporting: {OwningNPCTag}");
        }

        public string Log()
        {
            return "--";
        }
    }
}
