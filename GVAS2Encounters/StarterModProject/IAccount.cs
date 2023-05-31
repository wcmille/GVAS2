using System.Text;

namespace GVA.NPCControl
{
    public interface IAccount
    {
        void AddUnspent();
        bool RemoveUnspent();
        void Display(StringBuilder builder);
        void Read();
        void Write();
        void TimePeriod();

        string Log();

        string ColorFaction { get; }
        string OwningPCTag { get; }
        string OwningNPCTag { get; }
    }

    public interface IZoneFaction
    {
        bool BuyCivilian(int amt = 1);
        bool BuyMilitary(int amt = 1);

    }

}
