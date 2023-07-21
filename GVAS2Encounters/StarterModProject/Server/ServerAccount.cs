using Sandbox.ModAPI;
using System;
using VRage.Game.ModAPI;
using VRage.Utils;

namespace GVA.NPCControl.Server
{
    public interface IAntagonist
    {
        int Military { get; }

        /// <summary>
        /// Fight the other account.
        /// </summary>
        /// <returns>
        /// true is fight was lost
        /// </returns>
        /// <param name="account"></param>
        bool Fight();
    }

    public interface IServerAccount : IAccount
    {
        void TimePeriod(IAntagonist pirates = null);
    }

    public class ServerAccount : Accounting
    {
        const double corruption = 0.9;
        public ServerLog AccountLog { get; private set; }

        public ServerAccount(string color, ServerLog myLog) : base(color)
        {
            AccountLog = myLog;
            int incur;
            MyAPIGateway.Utilities.GetVariable($"{color}{SharedConstants.IncursionsStr}", out incur);
            Incursions = incur;
        }

        public ServerAccount(string owner, string npc, string f, int c, int m, double uu) : base(owner, npc, f, c, m, uu)
        {
        }

        public int Incursions { get; private set; }

        private string Log()
        {
            return $"Civ: {Civilian} Mil: {Military} Unspent: {UnspentUnits:F2}";
        }

        private void ResolveEconomy()
        {
            double netIncome = CalcNetIncome();

            UnspentUnits += netIncome;
            if (UnspentUnits < 0.0)
            {
                int balance = (int)Math.Ceiling(-UnspentUnits);
                if (Military >= balance)
                {
                    Military -= balance;
                    UnspentUnits = 0.0;
                }
                else
                {
                    Military = 0;
                    Civilian--;
                    UnspentUnits = 1.0;
                }

            }
            if (UnspentUnits > 50.0)
            {
                UnspentUnits *= corruption;
            }
            if (OwningPCTag == "SPRT")
            {
                Military /= 2;
                Civilian /= 2;
                UnspentUnits /= 2;
            }
        }

        private void ResolveIncursions(IAntagonist pirates)
        {
            //Resolve Incursions from the last period.
            for (int i = 0; i < Incursions; ++i)
            {
                if (pirates.Fight())
                {
                    Civilian--;
                    AccountLog.Log("Lost civilian building to pirates.");
                }
                else
                {
                    AccountLog.Log("Defended area against pirates.");
                }
            }
            //Resolve Incursions from the new period.
            {
                Incursions = (pirates.Military - Military) / 3;
                Incursions = Math.Min(Civilian / 2, Incursions);
                Incursions = Math.Max(0, Incursions);
            }
        }

        public void TimePeriod(IAntagonist pirates = null)
        {
            if (pirates != null)
            {
                ResolveIncursions(pirates);
            }
            else
            {
                MyLog.Default.WriteLine("SATDISH: Error Null Pirates");
            }
            ResolveEconomy();
            AccountLog.Log(Log());
        }

        public override void Read()
        {
            int incur;
            base.Read();
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.IncursionsStr}", out incur);
            Incursions = incur;
        }

        public override void Write()
        {
            base.Write();
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.IncursionsStr}", Incursions);
        }

        internal void LogSpawn(IMyCubeGrid grid)
        {
            AccountLog.Log(grid);
        }
    }
}
