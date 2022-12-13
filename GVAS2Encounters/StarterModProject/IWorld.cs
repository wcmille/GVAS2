using System.Collections.Generic;

namespace GVA.NPCControl
{
    public interface IWorld
    {
        Accounting GetAccountByColor(string color);
        void Write(Accounting acct);

        Accounting GetAccountByPCOwner(string factionTag);
    }

    public abstract class SharedWorld : IWorld
    {
        protected readonly List<Accounting> list = new List<Accounting>();

        public SharedWorld()
        {
            var blue = new Accounting(SharedConstants.BlueFactionColor);
            list.Add(blue);
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

        public abstract void Write(Accounting acct);
    }
}
