using System;
using System.Collections.Generic;
using VRage.Utils;

namespace GVA.NPCControl.Client
{
    public class ClientWorld : SharedWorld
    {
        readonly Client client;
        readonly ClientPirateAccount black;

        public ClientWorld(Client client)
        {
            try
            {
                var blue = new ClientAccount(SharedConstants.BlueFactionColor);
                list.Add(blue);
            }
            catch(Exception ex)
            {
                MyLog.Default.WriteLine("GVAS2: Failed to make blue.");
                MyLog.Default.WriteLine(ex.Message);
                MyLog.Default.WriteLine(ex.StackTrace);
            }
            try
            {
                var red = new ClientAccount(SharedConstants.RedFactionColor);
                list.Add(red);
            }
            catch(Exception ex)
            {
                MyLog.Default.WriteLine("GVAS2: Failed to make red.");
                MyLog.Default.WriteLine(ex.Message);
                MyLog.Default.WriteLine(ex.StackTrace);
            }
            try
            {
                black = new ClientPirateAccount();
                list.Add(black);
            }
            catch (Exception ex)
            {
                MyLog.Default.WriteLine("GVAS2: Failed to make pirate.");
                MyLog.Default.WriteLine(ex.Message);
                MyLog.Default.WriteLine(ex.StackTrace);
            }

            this.client = client;
        }

        public void GetAccountByPCOwner(string factionTag, List<IAccount> accounts)
        {
            accounts.Clear();
            foreach (IAccount f in list)
            {
                if (f.OwningPCTag == factionTag) accounts.Add(f);
            }
            if (accounts.Count == 0) accounts.Add(black);
        }

        public override void RequestReport(ulong requestor, IAccount ac)
        {
            client.Send(new CommandPacket(ac.OwningPCTag, ac.ColorFaction, SharedConstants.ReportCommand));
        }
    }
}
