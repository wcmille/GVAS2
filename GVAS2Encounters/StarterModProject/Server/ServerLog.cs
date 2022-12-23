using Sandbox.ModAPI;
using System;
using System.Linq;
using VRage.Game.ModAPI;

namespace GVA.NPCControl.Server
{
    public class ServerLog
    {
        readonly string filename;
        readonly int maxLogCount = 50;
        public string LogText { get; private set; }

        public ServerLog(string color, int maxLogCount = 50)
        {
            this.maxLogCount = maxLogCount;
            filename = $"{color}-MESLog.txt";
            LogText = "";
        }

        public void Log(IMyCubeGrid grid)
        {
            var pos = grid.GetPosition();
            Sandbox.ModAPI.Ingame.MyWaypointInfo mp = new Sandbox.ModAPI.Ingame.MyWaypointInfo(grid.CustomName, pos);
            LogText += $"{DateTime.UtcNow},{mp}\n";
            Write();
        }

        /// <summary>
        /// Read the log from disk.
        /// </summary>
        public void Read()
        {
            if (MyAPIGateway.Utilities.FileExistsInWorldStorage(filename, typeof(ServerLog)))
            {
                using (var file = MyAPIGateway.Utilities.ReadFileInWorldStorage(filename, typeof(ServerLog)))
                {
                    LogText = file.ReadToEnd();
                }
            }
        }

        private void CutTimes()
        {
            var split = LogText.Split('\n');
            LogText = string.Join("\n", split.Skip(Math.Min(split.Count() - maxLogCount, 0)));
        }

        /// <summary>
        /// Write the log to disk.
        /// </summary>
        public void Write()
        {
            using (var file = MyAPIGateway.Utilities.WriteFileInWorldStorage(filename, typeof(ServerLog)))
            {
                file.Write(LogText);
            }
        }
    }
}
