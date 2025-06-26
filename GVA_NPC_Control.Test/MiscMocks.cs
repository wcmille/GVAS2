using Digi.Example_NetworkProtobuf;
using GVA.NPCControl.Server;
using System;
using System.Collections.Generic;
using System.IO;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.Game.ModAPI.Interfaces;
using VRage.Library.Utils;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;

namespace GVA_NPC_Control.Test
{
    public class MockServer : IFactionsApi
    {
        public IMyFactionCollection Mfc { get; set; }

        public void SetReputation(long playerID, long factionID, int reputation)
        {
            Mfc.SetReputationBetweenPlayerAndFaction(playerID, factionID, reputation);
        }
    }
    public class MockPlayerCollection : IMyPlayerCollection
    {
        public long Count => throw new NotImplementedException();

        public event Action<IMyCharacter, MyDefinitionId> ItemConsumed;

        public void ExtendControl(IMyControllableEntity entityWithControl, IMyEntity entityGettingControl)
        {
            throw new NotImplementedException();
        }

        public void GetAllIdentites(List<IMyIdentity> identities, Func<IMyIdentity, bool> collect = null)
        {
            throw new NotImplementedException();
        }

        public IMyPlayer GetPlayerControllingEntity(IMyEntity entity)
        {
            throw new NotImplementedException();
        }

        public void GetPlayers(List<IMyPlayer> players, Func<IMyPlayer, bool> collect = null)
        {
            throw new NotImplementedException();
        }

        public bool HasExtendedControl(IMyControllableEntity firstEntity, IMyEntity secondEntity)
        {
            throw new NotImplementedException();
        }

        public void ReduceControl(IMyControllableEntity entityWhichKeepsControl, IMyEntity entityWhichLoosesControl)
        {
            throw new NotImplementedException();
        }

        public void RemoveControlledEntity(IMyEntity entity)
        {
            throw new NotImplementedException();
        }

        public void RequestChangeBalance(long identityId, long amount)
        {
            throw new NotImplementedException();
        }

        public void SetControlledEntity(ulong steamUserId, IMyEntity entity)
        {
            throw new NotImplementedException();
        }

        public void TryExtendControl(IMyControllableEntity entityWithControl, IMyEntity entityGettingControl)
        {
            throw new NotImplementedException();
        }

        public long TryGetIdentityId(ulong steamId)
        {
            throw new NotImplementedException();
        }

        public IMyPlayer TryGetIdentityId(long identity)
        {
            throw new NotImplementedException();
        }

        public ulong TryGetSteamId(long identityId)
        {
            return 1;
        }

        public bool TryReduceControl(IMyControllableEntity entityWhichKeepsControl, IMyEntity entityWhichLoosesControl)
        {
            throw new NotImplementedException();
        }
    }

    public class MockMultiPlayer : IMyMultiplayer
    {
        public bool IsServer => true;
        public bool IsClient => false;
        public bool IsReplay => false;
        public bool IsSinglePlayer => true;
        public ulong MyId { get; set; }

        public bool MultiplayerActive => throw new NotImplementedException();

        public ulong ServerId => throw new NotImplementedException();

        public string MyName => throw new NotImplementedException();

        public IMyPlayerCollection Players => throw new NotImplementedException();

        public event Action<IMyPlayer> PlayerJoined;
        public event Action<IMyPlayer> PlayerLeft;
        public event Action<IMyPlayer> PlayerRenamed;
        public event Action<IMyPlayer> PlayerBanned;
        public event Action<IMyPlayer> PlayerUnbanned;
        public event Action<IMyPlayer> PlayerKicked;

        public bool IsServerPlayer(IMyNetworkClient player)
        {
            throw new NotImplementedException();
        }

        public void JoinServer(string address)
        {
            throw new NotImplementedException();
        }

        public void RegisterMessageHandler(ushort id, Action<byte[]> messageHandler)
        {
            throw new NotImplementedException();
        }

        public void RegisterSecureMessageHandler(ushort id, Action<ushort, byte[], ulong, bool> messageHandler)
        {
            throw new NotImplementedException();
        }

        public void SendEntitiesCreated(List<MyObjectBuilder_EntityBase> objectBuilders)
        {
            throw new NotImplementedException();
        }

        public bool SendMessageTo(ushort id, byte[] message, ulong recipient, bool reliable = true)
        {
            throw new NotImplementedException();
        }

        public bool SendMessageToOthers(ushort id, byte[] message, bool reliable = true)
        {
            throw new NotImplementedException();
        }

        public bool SendMessageToServer(ushort id, byte[] message, bool reliable = true)
        {
            throw new NotImplementedException();
        }

        public void UnregisterMessageHandler(ushort id, Action<byte[]> messageHandler)
        {
            throw new NotImplementedException();
        }

        public void UnregisterSecureMessageHandler(ushort id, Action<ushort, byte[], ulong, bool> messageHandler)
        {
            throw new NotImplementedException();
        }
    }

    public class MockEntities : IMyEntities
    {
        public event Action<IMyEntity> OnEntityRemove;
        public event Action<IMyEntity> OnEntityAdd;
        public event Action OnCloseAll;
        public event Action<IMyEntity, string, string> OnEntityNameSet;

        public void AddEntity(IMyEntity entity, bool insertIntoScene = true)
        {
            throw new NotImplementedException();
        }

        public IMyEntity CreateFromObjectBuilder(MyObjectBuilder_EntityBase objectBuilder)
        {
            throw new NotImplementedException();
        }

        public IMyEntity CreateFromObjectBuilderAndAdd(MyObjectBuilder_EntityBase objectBuilder)
        {
            throw new NotImplementedException();
        }

        public IMyEntity CreateFromObjectBuilderNoinit(MyObjectBuilder_EntityBase objectBuilder)
        {
            throw new NotImplementedException();
        }

        public IMyEntity CreateFromObjectBuilderParallel(MyObjectBuilder_EntityBase objectBuilder, bool addToScene = false, Action<IMyEntity> completionCallback = null)
        {
            throw new NotImplementedException();
        }

        public void EnableEntityBoundingBoxDraw(IMyEntity entity, bool enable, Vector4? color = null, float lineWidth = 0.01F, Vector3? inflateAmount = null)
        {
            throw new NotImplementedException();
        }

        public bool EntityExists(string name)
        {
            throw new NotImplementedException();
        }

        public bool EntityExists(long entityId)
        {
            throw new NotImplementedException();
        }

        public bool EntityExists(long? entityId)
        {
            throw new NotImplementedException();
        }

        public bool Exist(IMyEntity entity)
        {
            throw new NotImplementedException();
        }

        public Vector3D? FindFreePlace(Vector3D basePos, float radius, int maxTestCount = 20, int testsPerDistance = 5, float stepSize = 1)
        {
            throw new NotImplementedException();
        }

        public List<IMyEntity> GetElementsInBox(ref BoundingBoxD boundingBox)
        {
            throw new NotImplementedException();
        }

        public void GetEntities(HashSet<IMyEntity> entities, Func<IMyEntity, bool> collect = null)
        {
            //throw new NotImplementedException();
        }

        public List<IMyEntity> GetEntitiesInAABB(ref BoundingBoxD boundingBox)
        {
            throw new NotImplementedException();
        }

        public List<IMyEntity> GetEntitiesInSphere(ref BoundingSphereD boundingSphere)
        {
            throw new NotImplementedException();
        }

        public IMyEntity GetEntity(Func<IMyEntity, bool> match)
        {
            throw new NotImplementedException();
        }

        public IMyEntity GetEntityById(long entityId)
        {
            throw new NotImplementedException();
        }

        public IMyEntity GetEntityById(long? entityId)
        {
            throw new NotImplementedException();
        }

        public IMyEntity GetEntityByName(string name)
        {
            throw new NotImplementedException();
        }

        public void GetInflatedPlayerBoundingBox(ref BoundingBox playerBox, float inflation)
        {
            throw new NotImplementedException();
        }

        public void GetInflatedPlayerBoundingBox(ref BoundingBoxD playerBox, float inflation)
        {
            throw new NotImplementedException();
        }

        public IMyEntity GetIntersectionWithSphere(ref BoundingSphereD sphere)
        {
            throw new NotImplementedException();
        }

        public IMyEntity GetIntersectionWithSphere(ref BoundingSphereD sphere, IMyEntity ignoreEntity0, IMyEntity ignoreEntity1)
        {
            throw new NotImplementedException();
        }

        public IMyEntity GetIntersectionWithSphere(ref BoundingSphereD sphere, IMyEntity ignoreEntity0, IMyEntity ignoreEntity1, bool ignoreVoxelMaps, bool volumetricTest, bool excludeEntitiesWithDisabledPhysics = false, bool ignoreFloatingObjects = true, bool ignoreHandWeapons = true)
        {
            throw new NotImplementedException();
        }

        public List<IMyEntity> GetIntersectionWithSphere(ref BoundingSphereD sphere, IMyEntity ignoreEntity0, IMyEntity ignoreEntity1, bool ignoreVoxelMaps, bool volumetricTest)
        {
            throw new NotImplementedException();
        }

        public List<IMyEntity> GetTopMostEntitiesInBox(ref BoundingBoxD boundingBox)
        {
            throw new NotImplementedException();
        }

        public List<IMyEntity> GetTopMostEntitiesInSphere(ref BoundingSphereD boundingSphere)
        {
            throw new NotImplementedException();
        }

        public bool IsInsideVoxel(Vector3 pos, Vector3 hintPosition, out Vector3 lastOutsidePos)
        {
            throw new NotImplementedException();
        }

        public bool IsInsideVoxel(Vector3D pos, Vector3D hintPosition, out Vector3D lastOutsidePos)
        {
            throw new NotImplementedException();
        }

        public bool IsInsideWorld(Vector3D pos)
        {
            throw new NotImplementedException();
        }

        public bool IsNameExists(IMyEntity entity, string name)
        {
            throw new NotImplementedException();
        }

        public bool IsRaycastBlocked(Vector3D pos, Vector3D target)
        {
            throw new NotImplementedException();
        }

        public bool IsSpherePenetrating(ref BoundingSphereD bs)
        {
            throw new NotImplementedException();
        }

        public bool IsTypeHidden(Type type)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(IMyEntity entity)
        {
            throw new NotImplementedException();
        }

        public bool IsWorldLimited()
        {
            throw new NotImplementedException();
        }

        public void MarkForClose(IMyEntity entity)
        {
            throw new NotImplementedException();
        }

        public void RegisterForDraw(IMyEntity entity)
        {
            throw new NotImplementedException();
        }

        public void RegisterForUpdate(IMyEntity entity)
        {
            throw new NotImplementedException();
        }

        public void RemapObjectBuilder(MyObjectBuilder_EntityBase objectBuilder)
        {
            throw new NotImplementedException();
        }

        public void RemapObjectBuilderCollection(IEnumerable<MyObjectBuilder_EntityBase> objectBuilders)
        {
            throw new NotImplementedException();
        }

        public void RemoveEntity(IMyEntity entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveFromClosedEntities(IMyEntity entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveName(IMyEntity entity)
        {
            throw new NotImplementedException();
        }

        public void SetEntityName(IMyEntity IMyEntity, bool possibleRename = true)
        {
            throw new NotImplementedException();
        }

        public void SetTypeHidden(Type type, bool hidden)
        {
            throw new NotImplementedException();
        }

        public bool TryGetEntityById(long id, out IMyEntity entity)
        {
            throw new NotImplementedException();
        }

        public bool TryGetEntityById(long? id, out IMyEntity entity)
        {
            throw new NotImplementedException();
        }

        public bool TryGetEntityByName(string name, out IMyEntity entity)
        {
            throw new NotImplementedException();
        }

        public void UnhideAllTypes()
        {
            throw new NotImplementedException();
        }

        public void UnregisterForDraw(IMyEntity entity)
        {
            throw new NotImplementedException();
        }

        public void UnregisterForUpdate(IMyEntity entity, bool immediate = false)
        {
            throw new NotImplementedException();
        }

        public float WorldHalfExtent()
        {
            throw new NotImplementedException();
        }

        public float WorldSafeHalfExtent()
        {
            throw new NotImplementedException();
        }
    }

    public class MockFactionCollection : IMyFactionCollection
    {
        public MockFactionCollection()
        {
            Factions = new Dictionary<long, IMyFaction>();
        }
        public Dictionary<long, IMyFaction> Factions { get; set; }

        //player, faction, reputation.
        readonly Dictionary<long, Dictionary<long, int>> reputation = new Dictionary<long, Dictionary<long, int>>();
        const int defaultRep = -1000;

        public event Action<long, bool, bool> FactionAutoAcceptChanged;
        public event Action<long> FactionEdited;
        public event Action<long> FactionCreated;
        public event Action<MyFactionStateChange, long, long, long, long> FactionStateChanged;
        public event Action<long, long, int, ReputationChangeReason> ReputationChanged;

        public void AcceptJoin(long factionId, long playerId)
        {
            throw new NotImplementedException();
        }

        public void AcceptPeace(long fromFactionId, long toFactionId)
        {
            throw new NotImplementedException();
        }

        public void AddNewNPCToFaction(long factionId)
        {
            throw new NotImplementedException();
        }

        public void AddNewNPCToFaction(long factionId, string npcName)
        {
            throw new NotImplementedException();
        }

        public void AddPlayerToFaction(long playerId, long factionId)
        {
            throw new NotImplementedException();
        }

        public bool AreFactionsEnemies(long factionId1, long factionId2)
        {
            throw new NotImplementedException();
        }

        public void CancelJoinRequest(long factionId, long playerId)
        {
            throw new NotImplementedException();
        }

        public void CancelPeaceRequest(long fromFactionId, long toFactionId)
        {
            throw new NotImplementedException();
        }

        public void ChangeAutoAccept(long factionId, long playerId, bool autoAcceptMember, bool autoAcceptPeace)
        {
            throw new NotImplementedException();
        }

        public void CreateFaction(long founderId, string tag, string name, string desc, string privateInfo)
        {
            throw new NotImplementedException();
        }

        public void CreateFaction(long founderId, string tag, string name, string desc, string privateInfo, MyFactionTypes type)
        {
            throw new NotImplementedException();
        }

        public void CreateNPCFaction(string tag, string name, string desc, string privateInfo)
        {
            throw new NotImplementedException();
        }

        public void DeclareWar(long fromFactionId, long toFactionId)
        {
            throw new NotImplementedException();
        }

        public void DemoteMember(long factionId, long playerId)
        {
            throw new NotImplementedException();
        }

        public void EditFaction(long factionId, string tag, string name, string desc, string privateInfo)
        {
            throw new NotImplementedException();
        }

        public bool FactionNameExists(string name, IMyFaction doNotCheck = null)
        {
            throw new NotImplementedException();
        }

        public bool FactionTagExists(string tag, IMyFaction doNotCheck = null)
        {
            throw new NotImplementedException();
        }

        public MyObjectBuilder_FactionCollection GetObjectBuilder()
        {
            throw new NotImplementedException();
        }

        public MyRelationsBetweenFactions GetRelationBetweenFactions(long factionId1, long factionId2)
        {
            throw new NotImplementedException();
        }

        public int GetReputationBetweenFactions(long factionId1, long factionId2)
        {
            throw new NotImplementedException();
        }

        public int GetReputationBetweenPlayerAndFaction(long identityId, long factionId)
        {
            if (!reputation.TryGetValue(identityId, out Dictionary<long, int> dict))
            {
                dict = new Dictionary<long, int>();
                reputation[identityId] = dict;
            }
            if (!dict.TryGetValue(factionId, out int rep))
            {
                dict[factionId] = defaultRep;
                rep = defaultRep;
            }
            return rep;
        }

        public bool IsPeaceRequestStatePending(long myFactionId, long foreignFactionId)
        {
            throw new NotImplementedException();
        }

        public bool IsPeaceRequestStateSent(long myFactionId, long foreignFactionId)
        {
            throw new NotImplementedException();
        }

        public void KickMember(long factionId, long playerId)
        {
            throw new NotImplementedException();
        }

        public void KickPlayerFromFaction(long playerId)
        {
            throw new NotImplementedException();
        }

        public void MemberLeaves(long factionId, long playerId)
        {
            throw new NotImplementedException();
        }

        public void PromoteMember(long factionId, long playerId)
        {
            throw new NotImplementedException();
        }

        public void RemoveFaction(long factionId)
        {
            throw new NotImplementedException();
        }

        public void SendJoinRequest(long factionId, long playerId)
        {
            throw new NotImplementedException();
        }

        public void SendPeaceRequest(long fromFactionId, long toFactionId)
        {
            throw new NotImplementedException();
        }

        public void SetReputation(long fromFactionId, long toFactionId, int reputation)
        {
            throw new NotImplementedException();
        }

        public void SetReputationBetweenPlayerAndFaction(long identityId, long factionId, int rep)
        {
            if (!reputation.TryGetValue(identityId, out var dict))
            {
                dict = new Dictionary<long, int>();
                reputation[identityId] = dict;
            }
            reputation[identityId][factionId] = rep;
            //int rep;
            //if (!dict.TryGetValue(factionId, out rep))
            //{
            //    dict[factionId] = defaultRep;
            //    rep = defaultRep;
            //}
            //return defaultRep;
        }

        public IMyFaction TryGetFactionById(long factionId)
        {
            Factions.TryGetValue(factionId, out IMyFaction v);
            return v;
        }

        public IMyFaction TryGetFactionByName(string name)
        {
            throw new NotImplementedException();
        }

        public IMyFaction TryGetFactionByTag(string tag)
        {
            foreach (var faction in Factions.Values)
            {
                if (tag == faction.Tag) return faction;
            }
            return null;
        }

        public IMyFaction TryGetPlayerFaction(long playerId)
        {
            throw new NotImplementedException();
        }

        public void EditFaction(long factionId, string tag, string name, string desc, string privateInfo, string icon, Vector3 factionColor, Vector3 factionIconColor)
        {
            throw new NotImplementedException();
        }

        public void CreateFactionNew(long founderId, string tag, string name, string desc, string privateInfo, string type)
        {
            throw new NotImplementedException();
        }
    }

    public class MockSession : IMySession
    {
        public float AssemblerEfficiencyMultiplier => throw new NotImplementedException();

        public float AssemblerSpeedMultiplier => throw new NotImplementedException();

        public bool AutoHealing => throw new NotImplementedException();

        public uint AutoSaveInMinutes => throw new NotImplementedException();

        public IMyCameraController CameraController => throw new NotImplementedException();

        public bool CargoShipsEnabled => throw new NotImplementedException();

        public bool ClientCanSave => throw new NotImplementedException();

        public bool CreativeMode => throw new NotImplementedException();

        public string CurrentPath => throw new NotImplementedException();

        public string Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IMyCamera Camera => throw new NotImplementedException();

        public double CameraTargetDistance { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IMyPlayer LocalHumanPlayer => throw new NotImplementedException();

        public IMyWeatherEffects WeatherEffects => throw new NotImplementedException();

        public IMyConfig Config => throw new NotImplementedException();

        public TimeSpan ElapsedPlayTime => throw new NotImplementedException();

        public bool EnableCopyPaste => throw new NotImplementedException();

        public MyEnvironmentHostilityEnum EnvironmentHostility => throw new NotImplementedException();

        public DateTime GameDateTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public float GrinderSpeedMultiplier => throw new NotImplementedException();

        public float HackSpeedMultiplier => throw new NotImplementedException();

        public float InventoryMultiplier => throw new NotImplementedException();

        public float CharactersInventoryMultiplier => throw new NotImplementedException();

        public float BlocksInventorySizeMultiplier => throw new NotImplementedException();

        public bool IsCameraAwaitingEntity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<MyObjectBuilder_Checkpoint.ModItem> Mods { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsCameraControlledObject => throw new NotImplementedException();

        public bool IsCameraUserControlledSpectator => throw new NotImplementedException();

        public bool IsServer => throw new NotImplementedException();

        public short MaxFloatingObjects => throw new NotImplementedException();

        public short MaxBackupSaves => throw new NotImplementedException();

        public short MaxPlayers => throw new NotImplementedException();

        public bool MultiplayerAlive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool MultiplayerDirect { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double MultiplayerLastMsg { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float NegativeIntegrityTotal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MyOnlineModeEnum OnlineMode => throw new NotImplementedException();

        public string Password { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float PositiveIntegrityTotal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public float RefinerySpeedMultiplier => throw new NotImplementedException();

        public bool ShowPlayerNamesOnHud => throw new NotImplementedException();

        public bool SurvivalMode => throw new NotImplementedException();

        public bool ThrusterDamage => throw new NotImplementedException();

        public string ThumbPath => throw new NotImplementedException();

        public TimeSpan TimeOnBigShip => throw new NotImplementedException();

        public TimeSpan TimeOnFoot => throw new NotImplementedException();

        public TimeSpan TimeOnJetpack => throw new NotImplementedException();

        public TimeSpan TimeOnSmallShip => throw new NotImplementedException();

        public bool WeaponsEnabled => throw new NotImplementedException();

        public float WelderSpeedMultiplier => throw new NotImplementedException();

        public ulong? WorkshopId => throw new NotImplementedException();

        public IMyVoxelMaps VoxelMaps => throw new NotImplementedException();

        public IMyPlayer Player => throw new NotImplementedException();

        public IMyControllableEntity ControlledObject => throw new NotImplementedException();

        public MyObjectBuilder_SessionSettings SessionSettings => throw new NotImplementedException();

        public IMyFactionCollection Factions { get; set; }

        public IMyDamageSystem DamageSystem => throw new NotImplementedException();

        public IMyGpsCollection GPS => throw new NotImplementedException();

        public BoundingBoxD WorldBoundaries => throw new NotImplementedException();

        public MyPromoteLevel PromoteLevel => throw new NotImplementedException();

        public bool HasCreativeRights => throw new NotImplementedException();

        public bool HasAdminPrivileges => throw new NotImplementedException();

        public Version Version => throw new NotImplementedException();

        public IMyOxygenProviderSystem OxygenProviderSystem => throw new NotImplementedException();

        public int GameplayFrameCounter => throw new NotImplementedException();

        public int TotalBotLimit => throw new NotImplementedException();

        public event Action OnSessionReady;
        public event Action OnSessionLoading;

        public void BeforeStartComponents()
        {
            throw new NotImplementedException();
        }

        public void Draw()
        {
            throw new NotImplementedException();
        }

        public void GameOver()
        {
            throw new NotImplementedException();
        }

        public void GameOver(MyStringId? customMessage)
        {
            throw new NotImplementedException();
        }

        public MyObjectBuilder_Checkpoint GetCheckpoint(string saveName)
        {
            throw new NotImplementedException();
        }

        public MyObjectBuilder_Sector GetSector()
        {
            throw new NotImplementedException();
        }

        public MyPromoteLevel GetUserPromoteLevel(ulong steamId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, byte[]> GetVoxelMapsArray()
        {
            throw new NotImplementedException();
        }

        public MyObjectBuilder_World GetWorld()
        {
            throw new NotImplementedException();
        }

        public bool IsPausable()
        {
            throw new NotImplementedException();
        }

        public bool IsUserAdmin(ulong steamId)
        {
            throw new NotImplementedException();
        }

        public bool IsUserIgnorePCULimit(ulong steamId)
        {
            throw new NotImplementedException();
        }

        public bool IsUserIgnoreSafeZones(ulong steamId)
        {
            throw new NotImplementedException();
        }

        public bool IsUserInvulnerable(ulong steamId)
        {
            throw new NotImplementedException();
        }

        public bool IsUserKeepOriginalOwnershipOnPaste(ulong steamId)
        {
            throw new NotImplementedException();
        }

        public bool IsUserPromoted(ulong steamId)
        {
            throw new NotImplementedException();
        }

        public bool IsUserShowAllPlayers(ulong steamId)
        {
            throw new NotImplementedException();
        }

        public bool IsUserUntargetable(ulong steamId)
        {
            throw new NotImplementedException();
        }

        public bool IsUserUseAllTerminals(ulong steamId)
        {
            throw new NotImplementedException();
        }

        public void RegisterComponent(MySessionComponentBase component, MyUpdateOrder updateOrder, int priority)
        {
            throw new NotImplementedException();
        }

        public bool Save(string customSaveName = null)
        {
            throw new NotImplementedException();
        }

        public void SetAsNotReady()
        {
            throw new NotImplementedException();
        }

        public void SetCameraController(MyCameraControllerEnum cameraControllerEnum, IMyEntity cameraEntity = null, Vector3D? position = null)
        {
            throw new NotImplementedException();
        }

        public void SetComponentUpdateOrder(MySessionComponentBase component, MyUpdateOrder order)
        {
            throw new NotImplementedException();
        }

        public bool TryGetAdminSettings(ulong steamId, out MyAdminSettingsEnum adminSettings)
        {
            throw new NotImplementedException();
        }

        public void Unload()
        {
            throw new NotImplementedException();
        }

        public void UnloadDataComponents()
        {
            throw new NotImplementedException();
        }

        public void UnloadMultiplayer()
        {
            throw new NotImplementedException();
        }

        public void UnregisterComponent(MySessionComponentBase component)
        {
            throw new NotImplementedException();
        }

        public void Update(MyTimeSpan time)
        {
            throw new NotImplementedException();
        }

        public void UpdateComponents()
        {
            throw new NotImplementedException();
        }
    }

    public class MockUtilities : IMyUtilities
    {
        public IMyConfigDedicated ConfigDedicated => throw new NotImplementedException();

        public IMyGamePaths GamePaths => throw new NotImplementedException();

        public bool IsDedicated => throw new NotImplementedException();

        public event MessageEnteredDel MessageEntered;
        public event MessageEnteredSenderDel MessageEnteredSender;
        public event Action<ulong, string> MessageRecieved;

        public IMyHudNotification CreateNotification(string message, int disappearTimeMs = 2000, string font = "White")
        {
            throw new NotImplementedException();
        }

        public void DeleteFileInGlobalStorage(string file)
        {
            throw new NotImplementedException();
        }

        public void DeleteFileInLocalStorage(string file, Type callingType)
        {
            throw new NotImplementedException();
        }

        public void DeleteFileInWorldStorage(string file, Type callingType)
        {
            throw new NotImplementedException();
        }

        public bool FileExistsInGameContent(string file)
        {
            throw new NotImplementedException();
        }

        public bool FileExistsInGlobalStorage(string file)
        {
            throw new NotImplementedException();
        }

        public bool FileExistsInLocalStorage(string file, Type callingType)
        {
            throw new NotImplementedException();
        }

        public bool FileExistsInModLocation(string file, MyObjectBuilder_Checkpoint.ModItem modItem)
        {
            throw new NotImplementedException();
        }

        public bool FileExistsInWorldStorage(string file, Type callingType)
        {
            return false;
        }

        public IMyHudObjectiveLine GetObjectiveLine()
        {
            throw new NotImplementedException();
        }

        public string GetTypeName(Type type)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> sandboxPairs = new Dictionary<string, string>();

        public bool GetVariable<T>(string name, out T value)
        {
            if (sandboxPairs.TryGetValue(name, out string s))
            {
                value = (T)Convert.ChangeType(s, typeof(T));
                return true;
            }
            else
            {
                T val = default;
                value = val;
                return false;
            }
        }

        public void InvokeOnGameThread(Action action, string invokerName = "ModAPI", int StartAt = -1, int RepeatTimes = 0)
        {
            throw new NotImplementedException();
        }

        public BinaryReader ReadBinaryFileInGameContent(string file)
        {
            throw new NotImplementedException();
        }

        public BinaryReader ReadBinaryFileInGlobalStorage(string file)
        {
            throw new NotImplementedException();
        }

        public BinaryReader ReadBinaryFileInLocalStorage(string file, Type callingType)
        {
            throw new NotImplementedException();
        }

        public BinaryReader ReadBinaryFileInModLocation(string file, MyObjectBuilder_Checkpoint.ModItem modItem)
        {
            throw new NotImplementedException();
        }

        public BinaryReader ReadBinaryFileInWorldStorage(string file, Type callingType)
        {
            throw new NotImplementedException();
        }

        public TextReader ReadFileInGameContent(string file)
        {
            throw new NotImplementedException();
        }

        public TextReader ReadFileInGlobalStorage(string file)
        {
            throw new NotImplementedException();
        }

        public TextReader ReadFileInLocalStorage(string file, Type callingType)
        {
            throw new NotImplementedException();
        }

        public TextReader ReadFileInModLocation(string file, MyObjectBuilder_Checkpoint.ModItem modItem)
        {
            throw new NotImplementedException();
        }

        public TextReader ReadFileInWorldStorage(string file, Type callingType)
        {
            throw new NotImplementedException();
        }

        public void RegisterMessageHandler(long id, Action<object> messageHandler)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVariable(string name)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(string messageText)
        {
            throw new NotImplementedException();
        }

        public void SendModMessage(long id, object payload)
        {
            throw new NotImplementedException();
        }

        public T SerializeFromBinary<T>(byte[] data)
        {
            throw new NotImplementedException();
        }

        public T SerializeFromXML<T>(string buffer)
        {
            throw new NotImplementedException();
        }

        public byte[] SerializeToBinary<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public string SerializeToXML<T>(T objToSerialize)
        {
            throw new NotImplementedException();
        }

        public void SetVariable<T>(string name, T value)
        {
            sandboxPairs[name] = value.ToString();
        }

        public void ShowMessage(string sender, string messageText)
        {
            throw new NotImplementedException();
        }

        public void ShowMissionScreen(string screenTitle = null, string currentObjectivePrefix = null, string currentObjective = null, string screenDescription = null, Action<ResultEnum> callback = null, string okButtonCaption = null)
        {
            throw new NotImplementedException();
        }

        public void ShowNotification(string message, int disappearTimeMs = 2000, string font = "White")
        {
            throw new NotImplementedException();
        }

        public void UnregisterMessageHandler(long id, Action<object> messageHandler)
        {
            throw new NotImplementedException();
        }

        public BinaryWriter WriteBinaryFileInGlobalStorage(string file)
        {
            throw new NotImplementedException();
        }

        public BinaryWriter WriteBinaryFileInLocalStorage(string file, Type callingType)
        {
            throw new NotImplementedException();
        }

        public BinaryWriter WriteBinaryFileInWorldStorage(string file, Type callingType)
        {
            throw new NotImplementedException();
        }

        public TextWriter WriteFileInGlobalStorage(string file)
        {
            throw new NotImplementedException();
        }

        public TextWriter WriteFileInLocalStorage(string file, Type callingType)
        {
            throw new NotImplementedException();
        }

        public TextWriter WriteFileInWorldStorage(string file, Type callingType)
        {
            //throw new NotImplementedException();
            return new StringWriter();
        }
    }

    public class MockNetworking : INetworking
    {
        public void Register()
        {
        }

        public void RelayToClients(PacketBase packet, byte[] rawData = null)
        {
            //throw new NotImplementedException();
        }

        public void SendToPlayer(PacketBase packet, ulong steamId)
        {
            throw new NotImplementedException();
        }

        public void SendToServer(PacketBase packet)
        {
            throw new NotImplementedException();
        }

        public void Unregister()
        {
            throw new NotImplementedException();
        }
    }
}
