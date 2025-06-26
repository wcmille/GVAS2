using GVA.NPCControl.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sandbox.ModAPI;
using System.Collections.Generic;
using VRage.Game;

namespace GVA_NPC_Control.Test
{
    [TestClass]
    public class AccountingTest
    {
        MockFaction faction;
        readonly string facColor = "Blue";
        MockServer server;
        ServerLog serverLog;

        [TestInitialize]
        public void Setup()
        {
            faction = new MockFaction
            {
                Tag = "BMC"
            };
            server = new MockServer();
            serverLog = new ServerLog(facColor);
        }

        [TestMethod]
        public void TestMethodx2()
        {
            //ServerAccount acct = new ServerAccount(facColor, serverLog, server);
            ServerAccount acct = new ServerAccount(faction, null, "Blue", 4, 55, 0);
            acct.TimePeriod();

            Assert.AreEqual(4, acct.Civilian, "Civilian");
            Assert.AreEqual(45, acct.Military, "Military");
            Assert.AreEqual(0.0, acct.UnspentUnits, 0.001);
        }

        [TestMethod]
        public void TestMethodx1()
        {
            ServerAccount acct = new ServerAccount(faction, null, "Blue", 0, 13, 0);
            acct.TimePeriod();

            Assert.AreEqual(0, acct.Civilian, "Civilian");
            Assert.AreEqual(10, acct.Military, "Military");
            Assert.AreEqual(0.0, acct.UnspentUnits, 0.001);
        }

        [TestMethod]
        public void TestMethod10_10_0()
        {
            ServerAccount acct = new ServerAccount(faction, null, "Blue", 10, 10, 0);
            acct.TimePeriod();

            Assert.AreEqual(10, acct.Civilian);
            Assert.AreEqual(10, acct.Military);
            Assert.AreEqual(0.666, acct.UnspentUnits, 0.001);
        }

        [TestMethod]
        public void TestMethod10_10_0_Unowned()
        {
            ServerAccount acct = new ServerAccount(null, null, "Blue", 10, 10, 0);
            acct.TimePeriod();

            Assert.AreEqual(5, acct.Civilian);
            Assert.AreEqual(5, acct.Military);
            Assert.AreEqual(0.333, acct.UnspentUnits, 0.001);
        }

        [TestMethod]
        public void TestMethod30_10_0()
        {
            ServerAccount acct = new ServerAccount(faction, null, "Blue", 30, 10, 0);
            acct.TimePeriod();

            Assert.AreEqual(30, acct.Civilian);
            Assert.AreEqual(10, acct.Military);
            Assert.AreEqual(2, acct.UnspentUnits, 0.001);
        }

        [TestMethod]
        public void TestMethod50_0_0()
        {
            ServerAccount acct = new ServerAccount(faction, null, "Blue", 50, 0, 0);
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

        [TestMethod]
        public void TestReputationC15()
        {
            var ses = new MockSession();
            MyAPIGateway.Session = ses;
            long playerId = 1;

            var leader = new MyFactionMember
            {
                PlayerId = playerId,
            };
            var members = new Dictionary<long, MyFactionMember>
            {
                { playerId, leader }
            };
            faction = new MockFaction(members)
            {
                FactionId = 100
            };
            var npcFaction = new MockFaction
            {
                FactionId = 200
            };
            var mfc = new MockFactionCollection();
            mfc.Factions.Add(faction.FactionId, faction);
            mfc.Factions.Add(npcFaction.FactionId, npcFaction);
            ses.Factions = mfc;
            MockServer ms = new MockServer();
            ms.Mfc = mfc;

            ServerAccount acct = new ServerAccount(faction, npcFaction, "Blue", 15, 0, 0, ms);

            MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(playerId, npcFaction.FactionId, 501);
            acct.TimePeriod();
            var rep = MyAPIGateway.Session.Factions.GetReputationBetweenPlayerAndFaction(playerId, npcFaction.FactionId);

            Assert.AreEqual(531, rep);
        }
    }
}
