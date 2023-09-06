using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GVA_NPC_Control.Test
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void TestChangeName()
        {
            long a = 89374392874;
            long b = 45804398508;

            Assert.AreEqual(a, a ^ b ^ b);
            Assert.AreEqual(b, a ^ b ^ a);
        }
    }
}
