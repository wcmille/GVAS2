using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GVA.NPCControl
{
    public interface IAccount
    {
        void AddUnspent();
        void Read();
        void Write();
        void TimePeriod();

        int Civilian { get; }
        int Military { get; }
        double UnspentUnits { get; }
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
