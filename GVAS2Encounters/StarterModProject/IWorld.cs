using System.Collections.Generic;

namespace GVA.NPCControl
{
    public interface IWorld
    {
        Accounting GetAccountByColor(string color);
        void Write(Accounting acct);

        Accounting GetAccountByPCOwner(string factionTag);
    }

    public class SharedWorld : IWorld
    {
        protected readonly List<Accounting> list = new List<Accounting>();

        public SharedWorld()
        {
            var blue = new Accounting(SharedConstants.BlueFactionColor);
            list.Add(blue);
            var red = new Accounting(SharedConstants.RedFactionColor);
            list.Add(red);
        }

        public Accounting GetAccountByColor(string color)
        {
            foreach (var f in list)
            {
                if (f.ColorFaction == color) return f;
            }
            return null;
        }

        public Accounting GetAccountByPCOwner(string factionTag)
        {
            foreach (var f in list)
            {
                if (f.OwningPCTag == factionTag) return f;
            }
            return null;
        }

        public virtual void Write(Accounting acct)
        {
            if (acct == null) return;
            acct.Write();
            foreach (var a in list)
            {
                if (a == acct) break; //No need to copy.
                if (a.ColorFaction == acct.ColorFaction)
                {
                    a.Read();
                    break;
                }
            }
        }
    }
}
