using VRage.Game;
using VRage.Game.ModAPI;
using VRageMath;

namespace GVA_NPC_Control.Test
{
    public class MockFaction : IMyFaction
    {
        public long FactionId { get; set; }

        public string Tag { get; set; }

        public string Name => throw new System.NotImplementedException();

        public string Description => throw new System.NotImplementedException();

        public string PrivateInfo => throw new System.NotImplementedException();

        public int Score { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public float ObjectivePercentageCompleted { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public VRage.Utils.MyStringId? FactionIcon => throw new System.NotImplementedException();

        public bool AutoAcceptMember => throw new System.NotImplementedException();

        public bool AutoAcceptPeace => throw new System.NotImplementedException();

        public bool AcceptHumans => throw new System.NotImplementedException();

        public long FounderId { get; set; }

        public Vector3 CustomColor => throw new System.NotImplementedException();

        public Vector3 IconColor => throw new System.NotImplementedException();

        public VRage.Collections.DictionaryReader<long, MyFactionMember> Members {get;set;}

        public VRage.Collections.DictionaryReader<long, MyFactionMember> JoinRequests => throw new System.NotImplementedException();

        public string GetBalanceShortString()
        {
            throw new System.NotImplementedException();
        }

        public bool IsEnemy(long playerId)
        {
            throw new System.NotImplementedException();
        }

        public bool IsEveryoneNpc()
        {
            throw new System.NotImplementedException();
        }

        public bool IsFounder(long playerId)
        {
            throw new System.NotImplementedException();
        }

        public bool IsFriendly(long playerId)
        {
            throw new System.NotImplementedException();
        }

        public bool IsLeader(long playerId)
        {
            throw new System.NotImplementedException();
        }

        public bool IsMember(long playerId)
        {
            throw new System.NotImplementedException();
        }

        public bool IsNeutral(long playerId)
        {
            throw new System.NotImplementedException();
        }

        public void RequestChangeBalance(long amount)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetBalanceInfo(out long balance)
        {
            throw new System.NotImplementedException();
        }
    }

}
