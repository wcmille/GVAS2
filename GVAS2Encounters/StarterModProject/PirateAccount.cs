using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GVA.NPCControl
{
    public class PirateAccount : IAccount
    {
        public PirateAccount()
        {
            OwningNPCTag = SharedConstants.BlackFactionTag;
            ColorFaction = SharedConstants.BlackFactionColor;
        }
        public int Civilian { get; private set; }

        public int Military { get; private set; }

        public double UnspentUnits { get; private set; }

        public string ColorFaction { get; private set; }

        public string OwningNPCTag { get; private set; }
        public string OwningPCTag { get { return null; } }

        public void AddUnspent(/*PC Faction*/)
        {
            UnspentUnits += 1.0;
            //Increase everyone's rep in the PC faction.
        }

        public void TimePeriod()
        {
            //Decay everyone's rep.
            //Military goes up.
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
