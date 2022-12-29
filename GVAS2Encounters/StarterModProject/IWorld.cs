using System.Collections.Generic;

namespace GVA.NPCControl
{
    public interface IWorld
    {
        IAccount GetAccountByColor(string color);
        void Write(IAccount acct);

        void GetAccountByPCOwner(string factionTag, List<IAccount> accounts);
        void RequestReport(ulong requestor, IAccount ac);
    }
}
