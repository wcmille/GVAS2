using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GVA.NPCControl
{
    public interface IServerCommands
    {
        void Execute(string command);
    }

    public class ServerCommands : IServerCommands
    {
        public void Execute(string command)
        {
            SatDishLogic.IncreaseNPC("Blue", "Civilian");
        }
    }
}
