using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using MD.Character;
using MD.Network.GameMode;
using MD.AI;

namespace MD.UI
{
    public class NetworkManagerLobby : NetworkManager
    {
        #region CONSTANTS
        private readonly string NAME_PLAYER_ONLINE = "Player Online";
        private readonly string SPAWN_PLATFORM = "Spawn Platform";
        private readonly string DIGGABLE_GENERATOR = "Diggable Generator";
        private readonly string DIGGABLE_GENERATOR_COMMUNICATOR = "Diggable Generator Communicator";
        private readonly string SONAR = "Sonar";
        private readonly string MAP_GENERATOR = "Map Generator";
        private readonly string MAP_RENDERER = "Map Renderer";
        private readonly string SCAN_WAVE_SPAWNER = "Scan Wave Spawner";
        private readonly string OFFSCREEN_INDICATOR = "Offscreen Indicator";
        private readonly string STORAGE = "Storage";
        #endregion

        #region SERIALIZE FIELDS
        [Header("Scene")]
        [Scene] [SerializeField]
        private string menuScene = string.Empty;

        [Scene] [SerializeField]
        private string gamePlayScene = string.Empty;

        [SerializeField]
        private float matchTime = 240f;

        [Header("Room")]
        [SerializeField]
        private int maximumPlayers = 4;

        [SerializeField]
        private int minimumPlayers = 2;

        [SerializeField]
        private NetworkRoomPlayerLobby roomPlayerPrefab = null;

        [SerializeField]
        private GameObject botPrefab = null;
        #endregion

        #region FIELDS
        public string GameplayScene => gamePlayScene;
        public List<GameObject> DontDestroyOnLoadObjects = new List<GameObject>();
        private Player networkPlayerPrefab = null;
        public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
        public List<Player> Players { get; } = new List<Player>();
        public List<PlayerBot> Bots { get; } = new List<PlayerBot>();
        public bool isBotTraining;
        private IGameModeManager gameModeManager;
        private Map.Core.SpawnPositionsData spawnPositionsData;
        private List<uint> aliveBots = new List<uint>();
        private Map.Core.IMapGenerator mapGenerator;
        #endregion

        public static event Action OnClientConnected;
        public static event Action OnClientDisconnnected;

        public int MinNumPlayers => minimumPlayers; 
        public Vector2 NextSpawnPoint => spawnPositionsData.NextSpawnPoint;
        private Player NetworkPlayerPrefab
        {
            set => networkPlayerPrefab = value;
            get
            {
                if (networkPlayerPrefab != null) return networkPlayerPrefab;
                networkPlayerPrefab = spawnPrefabs.Find(prefab => prefab.name.Equals(NAME_PLAYER_ONLINE)).GetComponent<Player>();
                return networkPlayerPrefab;
            }
        }

        public override void OnStartServer() 
        {
            spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
        }

        public override void OnStartClient()
        {
            var spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");
            foreach (var prefab in spawnPrefabs)
            {
                ClientScene.RegisterPrefab(prefab);
            }
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            OnClientConnected?.Invoke();           
        }

        public void CleanObjectsOnDisconnect()
        {
            Debug.Log("Clean up on Disconnect");

            RoomPlayers.Clear();
            Players.Clear();
            ServiceLocator.Reset();

            foreach (GameObject obj in DontDestroyOnLoadObjects)
            {
                if (obj != null) NetworkServer.Destroy(obj);
            }

            Destroy(NetworkManager.singleton.gameObject);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            // Debug.Log("on client disconnect");
            // if (SceneManager.GetActiveScene().path == gamePlayScene)
            // {
            //     CleanObjectsWhenDisconnect();
            //     // SceneManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);
            // }
            base.OnClientDisconnect(conn);
            OnClientDisconnnected?.Invoke();
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            Debug.Log("Number of players: " + numPlayers);

            if (numPlayers == maximumPlayers || SceneManager.GetActiveScene().path != menuScene)
            {
                conn.Disconnect();
                return;
            }
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if (conn.identity != null)
            {
                if (SceneManager.GetActiveScene().path == menuScene)
                {
                    var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();
                    RoomPlayers.Remove(player);
                    NotifyReadyState();
                }
                else if (SceneManager.GetActiveScene().path == gamePlayScene)
                {
                    var player = conn.identity.GetComponent<Player>();
                    Players.Remove(player);
                }
            }

            base.OnServerDisconnect(conn);
        }

        public void RegisterGameModeManager(IGameModeManager gameModeManager) => this.gameModeManager = gameModeManager;

        public override void OnServerAddPlayer(NetworkConnection conn)
        {        
            if (!SceneManager.GetActiveScene().path.Equals(menuScene)) 
            {
                return;
            }
            
            gameModeManager.HandleOnServerAddPlayer(conn);    
        }

        public Player SpawnBotTrainingPlayer(NetworkConnection conn)
        {
            var player = Instantiate(NetworkPlayerPrefab);
            player.SetPlayerNameAndColor(PlayerPrefs.GetString(PlayerNameInput.PLAYER_PREF_NAME_KEY));
            NetworkServer.AddPlayerForConnection(conn, player.gameObject); 
            return player;        
        }

        public void SpawnRoomPlayer(NetworkConnection conn)
        {
            var isHost = RoomPlayers.Count == 0;
            var roomPlayer = Instantiate(roomPlayerPrefab);
            roomPlayer.IsHost = isHost;
            NetworkServer.AddPlayerForConnection(conn, roomPlayer.gameObject);
        }

        public void NotifyReadyState()
        {
            RoomPlayers.ForEach(roomPlayer => roomPlayer.HandleReadyToStart(IsReadyToStart()));
        }

        public override void OnStopServer()
        {
            Time.timeScale = 1f;
            ServerChangeScene(menuScene);
            CleanObjectsOnDisconnect();
            GetComponent<CustomNetworkDiscovery>().StopAdvertisingServer();
            base.OnStopServer();
        }

        public override void ServerChangeScene(string sceneName)
        {
            HandleServerChangeScene();
            base.ServerChangeScene(sceneName);
        }

        private void HandleServerChangeScene()
        {
            if (!SceneManager.GetActiveScene().path.Equals(menuScene))
            {
                return;
            }

            InitEnv();
            gameModeManager.HandleServerChangeScene();
        }
        private void InitEnv()
        {
            mapGenerator = SpawnMapGenerator();  
            spawnPositionsData = mapGenerator.SpawnPositionsData;
            SpawnDiggableGenerator();            
        }

        private void SpawnDiggableGenerator()
        {
            var diggableGeneratorGO = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals(DIGGABLE_GENERATOR)));
            NetworkServer.Spawn(diggableGeneratorGO);
            DontDestroyOnLoad(diggableGeneratorGO);
            DontDestroyOnLoadObjects.Add(diggableGeneratorGO);
        }

        private Map.Core.IMapGenerator SpawnMapGenerator()
        {
            var mapGeneratorGO = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals(MAP_GENERATOR)));
            NetworkServer.Spawn(mapGeneratorGO);
            DontDestroyOnLoad(mapGeneratorGO);
            DontDestroyOnLoadObjects.Add(mapGeneratorGO);
            return mapGeneratorGO.GetComponent<Map.Core.IMapGenerator>();
        }

        public void SpawnPvPPlayers()
        {
            RoomPlayers.ForEach((roomPlayer, idx) =>
            {
                var player = Instantiate(NetworkPlayerPrefab, spawnPositionsData.SpawnPositions[idx], Quaternion.identity);
                player.SetPlayerNameAndColor(roomPlayer.DisplayName);
                var conn = roomPlayer.netIdentity.connectionToClient;
                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, player.gameObject, true);
            });          
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (!SceneManager.GetActiveScene().path.Equals(gamePlayScene))
            {
                return;
            }
            
            SpawnSonar();            
            SpawnDiggableGeneratorCommunicator();  
            Players.ForEach(player => GenMapRenderer(player.connectionToClient));
            Players.ForEach((_, idx) => SpawnSpawnPlatform(idx));
            Players.ForEach(player => SpawnStorage(player.netIdentity, player.PlayerColor));

            SpawnScanWaveSpawner();
            //TODO check if all players loaded scene
            SetupGame();      
        }

        private void SpawnScanWaveSpawner()
        {
            var scanWaveSpawner = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals(SCAN_WAVE_SPAWNER)));
            NetworkServer.Spawn(scanWaveSpawner);
        } 

        private void SpawnOffScreenIndicator()
        {
            var offscreenIndicator = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals(OFFSCREEN_INDICATOR)));
            NetworkServer.Spawn(offscreenIndicator);
        }

        private void SpawnSpawnPlatform(int playerIdx)
        {
            var prefabToSpawn = spawnPrefabs.Find(prefab => prefab.name.Equals(SPAWN_PLATFORM));
            var platform = Instantiate(prefabToSpawn, spawnPositionsData.SpawnPositions[playerIdx] - new Vector2(0f, .4f), Quaternion.identity);
            NetworkServer.Spawn(platform);
        }

        private void SpawnStorage(NetworkIdentity playerId, Color flagColor)
        {
            // var storage = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals(STORAGE)), playerId.transform.position, Quaternion.identity);
            // int rnd = UnityEngine.Random.Range(0, storagePosList.Count);
            var storage = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals(STORAGE)), mapGenerator.SpawnStorage(), Quaternion.identity);
            storage.GetComponent<Diggable.Core.Storage>().Initialize(playerId, flagColor);
            NetworkServer.Spawn(storage.gameObject);
            // mapGenerator.UpdateObsatcleData(((int)storagePosList[rnd].x),((int)storagePosList[rnd].y));
            // storagePosList.RemoveAt(rnd);
        }

        private void SpawnSonar()
        {
            var sonar = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals(SONAR)));
            NetworkServer.Spawn(sonar);
        }

        private void SpawnDiggableGeneratorCommunicator()
        {
            var digGenComm = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals(DIGGABLE_GENERATOR_COMMUNICATOR)));
            NetworkServer.Spawn(digGenComm);
        }

        private void GenMapRenderer(NetworkConnection conn)
        {
            var mapRenderer = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals(MAP_RENDERER)));
            NetworkServer.Spawn(mapRenderer, conn);
        }

        public void StartGame()
        {
            if (SceneManager.GetActiveScene().path == menuScene && IsReadyToStart())
            {           
                if (ServiceLocator.Resolve<MD.VisualEffects.FadeScreen>(out MD.VisualEffects.FadeScreen fadeScreen))
                {
                    fadeScreen.StartFading(() => ServerChangeScene(gamePlayScene));
                    return;
                }

                ServerChangeScene(gamePlayScene);
            }
        } 

        public bool IsReadyToStart() => gameModeManager.IsReadyToStart();

        public GameObject SpawnBot(Vector2 spawnPos)
        {
            var bot = Instantiate(botPrefab, spawnPos, Quaternion.identity);
            Bots.Add(bot.GetComponent<PlayerBot>());
            NetworkServer.Spawn(bot, Players[0].connectionToClient);
            return bot;
        }

        private void SetupGame()
        {  
            gameModeManager.SetupGame(matchTime, Players);
            Invoke(nameof(EndGameByTimeOut), matchTime);
        }

        private void EndGameByTimeOut() => gameModeManager.EndGameByTimeOut(Players, Bots);

    #if UNITY_EDITOR
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CancelInvoke();
                EndGameByTimeOut();
            }
        }   
    #endif
    }
}    
