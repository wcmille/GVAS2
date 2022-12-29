using System.Collections.Generic;

namespace GVA.NPCControl
{
    public interface IWorld
    {
        IAccount GetAccountByColor(string color);
        void Write(IAccount acct);

        IAccount GetAccountByPCOwner(string factionTag);
        void RequestReport(ulong requestor, IAccount ac);
    }
}
