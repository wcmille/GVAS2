using Digi.Example_NetworkProtobuf;
using GVA.NPCControl;
using GVA.NPCControl.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sandbox.ModAPI;
using System.Collections.Generic;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.Utils;

namespace GVA_NPC_Control.Test
{
    [TestClass]
    public class ChangeOfFactionName
    {
        //Test cases:
        //Player leaves PC faction
        //Player gets kicked from PC faction
        //Player leaves other enemy faction for a neutral one.
        //Player leaves neutral faction for an enemy one.

        //Faction declares war
        //Faction declared war on.

        //Faction accepted peace
        //Faction accepts peace

        MockFaction bmcFaction, npcFaction, akiFaction;
        long playerId;
        MockFactionCollection factions;
        MockUtilities utilities;
        ServerWorld sw;

        readonly long jaghFounderId = 3001;

        [TestInitialize]
        public void Setup()
        {
            var pirates = new MockFaction
            {
                Tag = "SPRT",
                FactionId = 1000,
            };

            npcFaction = new MockFaction
            {
                Tag = "JAGH",
                FactionId = 2000,
            };

            bmcFaction = new MockFaction
            {
                Tag = "BMC",
                FactionId = 3000,
                FounderId = jaghFounderId,
            };

            var akiPlayer = 4001;
            var akiMembers = new Dictionary<long, MyFactionMember>();
            MyFactionMember mem = new MyFactionMember(akiPlayer, true, true);
            akiMembers.Add(akiPlayer, mem);
            akiFaction = new MockFaction
            {
                Tag = "AKI",
                FactionId = 4000,
                FounderId = mem.PlayerId,
                Members = akiMembers,
            };

            playerId = 3002;


            //Simulate Boot
            utilities = new MockUtilities();
            MyAPIGateway.Utilities = utilities;

            var session = new MockSession();
            MyAPIGateway.Session = session;

            factions = new MockFactionCollection();
            factions.Factions.Add(pirates.FactionId, pirates);
            session.Factions = factions;

            MyAPIGateway.Entities = new MockEntities();

            MyLog.Default = new MyLog();

            MyAPIGateway.Players = new MockPlayerCollection();
            
            var mmp = new MockMultiPlayer();
            mmp.MyId = 1; // Simulate the player being the server player
            MyAPIGateway.Multiplayer = mmp;

            //Simulate Files
            factions.Factions.Add(bmcFaction.FactionId, bmcFaction);
            factions.Factions.Add(npcFaction.FactionId, npcFaction);
            factions.Factions.Add(akiFaction.FactionId, akiFaction);
            utilities.SetVariable($"{SharedConstants.BlueFactionColor}{SharedConstants.OwnerId}", bmcFaction.FactionId);
            utilities.SetVariable($"{SharedConstants.BlueFactionColor}{SharedConstants.NPCStr}", npcFaction.Tag);
            factions.SetReputationBetweenPlayerAndFaction(bmcFaction.FounderId, npcFaction.FactionId, 1000);

            //Simulate Mod Code
            INetworking networking = new MockNetworking();
            Server server = new Server(networking);
            sw = new ServerWorld(server);
            var sa = (ServerAccount)sw.GetAccountByColor(SharedConstants.BlueFactionColor);
        }
        #region baseline tests
        [TestMethod]
        public void TestUtilityReader() 
        {
            const int goal = 99;
            var str = $"{SharedConstants.BlueFactionColor}{SharedConstants.MilitaryStr}";
            utilities.sandboxPairs[str] = goal.ToString();

            int mil;
            var result = MyAPIGateway.Utilities.GetVariable<int>(str, out mil);
            Assert.IsTrue(result, "GetVariable failed");
            Assert.AreEqual(goal, mil);
        }

        [TestMethod]
        public void TestChangeName()
        {
            string newTag = "FOO";
            ServerAccount acct = new ServerAccount(bmcFaction, null, "Blue", 4, 55, 0);
            bmcFaction.Tag = newTag;

            Assert.AreEqual(newTag, acct.OwningPCTag);
        }

        [TestMethod]
        public void NoChangesWhenActionIsntInteresting()
        {
            var action = MyFactionStateChange.CancelFriendRequest;
            sw.Factions_FactionStateChanged(action, 0, 0, 0, 0);
        }

        [TestMethod]
        public void FetchesBMC()
        {
            var f = factions.TryGetFactionById(bmcFaction.FactionId);
            Assert.AreEqual(bmcFaction, f);
        }

        [TestMethod]
        public void PlayerDefaultsToEnemyOfJagh()
        {
            Assert.AreEqual(-1000, factions.GetReputationBetweenPlayerAndFaction(playerId, npcFaction.FactionId));
        }
        #endregion

        [TestMethod]
        public void PlayerJoinsBMC()
        {
            var action = MyFactionStateChange.FactionMemberAcceptJoin;
            sw.Factions_FactionStateChanged(action, 0, bmcFaction.FactionId, playerId, 0);

            Assert.AreEqual(SharedConstants.AlliedRep, factions.GetReputationBetweenPlayerAndFaction(playerId, npcFaction.FactionId));
        }

        [TestMethod]
        public void BMCFriendlyIsJaghFriendly()
        {
            var action = MyFactionStateChange.AcceptPeace;
            sw.Factions_FactionStateChanged(action, akiFaction.FactionId, bmcFaction.FactionId, 0, 0);

            Assert.AreEqual(SharedConstants.DefaultRep, factions.GetReputationBetweenPlayerAndFaction(akiFaction.FounderId, npcFaction.FactionId));
        }

        [TestMethod]
        public void BMCFriendlyIsJaghFriendlyOtherWay()
        {
            var action = MyFactionStateChange.AcceptPeace;
            sw.Factions_FactionStateChanged(action, bmcFaction.FactionId, akiFaction.FactionId, 0, 0);

            Assert.AreEqual(SharedConstants.DefaultRep, factions.GetReputationBetweenPlayerAndFaction(akiFaction.FounderId, npcFaction.FactionId));
        }
    }
}
