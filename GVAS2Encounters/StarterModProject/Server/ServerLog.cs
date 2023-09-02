using Sandbox.ModAPI;
using System;
using System.Linq;
using VRage.Game.ModAPI;
using VRageMath;

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

        public void Log(IMyCubeGrid grid, Color color, string formatter)
        {
            var pos = grid.GetPosition();
            pos.X = Math.Round(pos.X);
            pos.Y = Math.Round(pos.Y);
            pos.Z = Math.Round(pos.Z);
            string name = grid.CustomName.Remove(0, 6);
            Sandbox.ModAPI.Ingame.MyWaypointInfo mp = new Sandbox.ModAPI.Ingame.MyWaypointInfo($"{formatter}{name}", pos);
            string hex = "#FF" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
            LogText += $"{DateTime.UtcNow},{mp}{hex}:\n";
            Write();
        }

        public void Log(string text)
        {
            LogText += $"{DateTime.UtcNow}, {text}\n";
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
            CutTimes();
            using (var file = MyAPIGateway.Utilities.WriteFileInWorldStorage(filename, typeof(ServerLog)))
            {
                file.Write(LogText);
            }
        }
    }
}
