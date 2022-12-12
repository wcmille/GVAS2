using System;

namespace GVA.NPCControl.Client
{
    public class ClientWorld : IWorld
    {
        public ClientWorld()
        {
        }

        public Accounting GetAccountDetails(string color)
        {
            return new Accounting(color);
        }

        public Accounting GetTerritoryOwner(string factionTag)
        {
            if (string.IsNullOrEmpty(factionTag))
            {
                return null;
            }
            return new Accounting(SharedConstants.BlueFactionColor);
        }

        public void Write(Accounting acct)
        {
            acct.Write();
        }
    }
}
