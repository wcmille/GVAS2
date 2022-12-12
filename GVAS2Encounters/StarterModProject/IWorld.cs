namespace GVA.NPCControl
{
    public interface IWorld
    {
        Accounting GetAccountDetails(string color);
        void Write(Accounting acct);

        Accounting GetTerritoryOwner(string factionTag);
    }
}
