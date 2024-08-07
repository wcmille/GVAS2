﻿using System.Collections.Generic;

namespace GVA.NPCControl
{
    public abstract class SharedWorld : IWorld
    {
        protected readonly List<IAccount> list = new List<IAccount>();

        public IAccount GetAccountByColor(string color)
        {
            foreach (var f in list)
            {
                if (f.ColorFaction == color) return f;
            }
            return null;
        }

        public IAccount GetAccountByNPCOwner(string factionTag)
        {
            foreach (var f in list)
            {
                if (f.OwningNPCTag == factionTag) return f;
            }
            return null;
        }

        public abstract void RequestReport(ulong requestor, IAccount ac);

        public virtual void Write(IAccount acct)
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
