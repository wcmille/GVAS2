using GVA.NPCControl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GVA_NPC_Control.Test
{
    [TestClass]
    public class AccountingTest
    {
        [TestMethod]
        public void TestMethodx1()
        {
            Accounting acct = new Accounting(null, null, "Blue", 0, 13, 0);
            acct.TimePeriod();

            Assert.AreEqual(0, acct.Civilian, "Civilian");
            Assert.AreEqual(10, acct.Military, "Military");
            Assert.AreEqual(0.0, acct.UnspentUnits, 0.001);
        }

        [TestMethod]
        public void TestMethod10_10_0()
        {
            Accounting acct = new Accounting(null,null,"Blue", 10, 10, 0);
            acct.TimePeriod();

            Assert.AreEqual(10, acct.Civilian);
            Assert.AreEqual(10, acct.Military);
            Assert.AreEqual(0.666, acct.UnspentUnits, 0.001);
        }

        [TestMethod]
        public void TestMethod10_10_0_Unowned()
        {
            Accounting acct = new Accounting("SPRT", null, "Blue", 10, 10, 0);
            acct.TimePeriod();

            Assert.AreEqual(5, acct.Civilian);
            Assert.AreEqual(5, acct.Military);
            Assert.AreEqual(0.333, acct.UnspentUnits, 0.001);
        }

        [TestMethod]
        public void TestMethod30_10_0()
        {
            Accounting acct = new Accounting(null, null, "Blue", 30, 10, 0);
            acct.TimePeriod();

            Assert.AreEqual(30, acct.Civilian);
            Assert.AreEqual(10, acct.Military);
            Assert.AreEqual(2, acct.UnspentUnits, 0.001);
        }

        [TestMethod]
        public void TestMethod50_0_0()
        {
            Accounting acct = new Accounting(null, null, "Blue", 50, 0, 0);
            acct.TimePeriod();

            Assert.AreEqual(49, acct.Civilian, "Civilian");
            Assert.AreEqual(1, acct.UnspentUnits, 0.001, "Unspent");
            Assert.AreEqual(0, acct.Military, "Military");
        }

        [TestMethod]
        public void CheckContains()
        {
            string thing = "Faction Territory Blue[NPC - IGNORE]";
            Assert.IsTrue(thing.Contains("Faction Territory Blue"));
        }
    }
}
