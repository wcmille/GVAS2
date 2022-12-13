using System;

namespace GVA.NPCControl.Client
{
    public class ClientWorld : SharedWorld
    {
        public ClientWorld()
        {
        }

        public override void Write(Accounting acct)
        {
            acct.Write();
        }
    }
}
