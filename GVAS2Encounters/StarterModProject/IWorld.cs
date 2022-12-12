namespace GVA.NPCControl
{
    public interface IWorld
    {
        Accounting GetAccountDetails(string color);
        void Write(Accounting acct);
    }
}
