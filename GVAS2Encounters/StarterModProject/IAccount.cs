namespace GVA.NPCControl
{
    public interface IAccount
    {
        void AddUnspent();
        bool RemoveUnspent();
        void Read();
        void Write();
        string ColorFaction { get; }
        string OwningPCTag { get; }
        string OwningNPCTag { get; }
    }
}
