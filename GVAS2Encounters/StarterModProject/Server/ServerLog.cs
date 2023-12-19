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
        readonly int maxLogCount = 500;
        public string LogText { get; private set; }

        public ServerLog(string color, int maxLogCount = 500)
        {
            this.maxLogCount = maxLogCount;
            filename = $"{color}-MESLog.txt";
            LogText = "";
        }

        public void Log(IMyCubeGrid grid, Color gpsColor, string formatter)
        {
            if (grid == null) throw new ArgumentNullException(nameof(grid));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            var pos = grid.GetPosition();
            string name = grid.CustomName;
            LogText += WriteLogLine(gpsColor, formatter+name, pos);
            Write();
        }

        internal string WriteLogLine(Color color, string formatter, Vector3D pos)
        {
            pos.X = Math.Round(pos.X);
            pos.Y = Math.Round(pos.Y);
            pos.Z = Math.Round(pos.Z);
            Sandbox.ModAPI.Ingame.MyWaypointInfo mp = new Sandbox.ModAPI.Ingame.MyWaypointInfo($"{formatter}", pos);
            string hex = $"#FF{color.R:X2}{color.G:X2}{color.B:X2}";
            return $"{DateTime.UtcNow},{mp}{hex}:\n";
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
            LogText = string.Join("\n", split.Skip(Math.Max(split.Count() - maxLogCount, 0)));
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
