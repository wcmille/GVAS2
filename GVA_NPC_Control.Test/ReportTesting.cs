using GVA.NPCControl.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VRageMath;

namespace GVA_NPC_Control.Test
{
    [TestClass]
    public class ReportTesting
    {
        [TestMethod]
        public void TestLogAndValues()
        {
            ServerLog sl = new ServerLog("Blue");
            StringAssert.EndsWith(sl.WriteLogLine(Color.Red, "FM", new Vector3D(1.0, 2.0, 3.0)), "GPS:FM:1:2:3:#FFFF0000:\n");
        }
    }
}
