namespace GVA.NPCControl
{
    public interface IWorld
    {
        IAccount GetAccountByColor(string color);
        void Write(IAccount acct);

        void RequestReport(ulong requestor, IAccount ac);
    }
}
