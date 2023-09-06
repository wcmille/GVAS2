using VRage.Game.ModAPI;

namespace GVA.NPCControl
{
    public interface IAccount
    {
        void AddUnspent(IMyFaction donor);
        bool RemoveUnspent();
        void Read();
        void Write();
        string ColorFaction { get; }
        string OwningPCTag { get; }
        IMyFaction OwningPCFaction { get; }
        string OwningNPCTag { get; }
    }
}
