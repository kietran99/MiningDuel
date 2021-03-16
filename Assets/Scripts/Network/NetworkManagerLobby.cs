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
        private readonly string DIGGABLE_GENERATOR = "Diggable Generator";
        private readonly string DIGGABLE_GENERATOR_COMMUNICATOR = "Diggable Generator Communicator";
        private readonly string SONAR = "Sonar";
        private readonly string MAP_GENERATOR = "Map Generator";
        private readonly string MAP_RENDERER = "Map Renderer";
        private readonly string SCAN_WAVE_SPAWNER = "Scan Wave Spawner";
        #endregion

        #region SERIALIZE FIELDS
        [Header("Scene")]
        [Scene] [SerializeField]
        private string menuScene = string.Empty;

        [Scene] [SerializeField]
        private string gamePlayScene = string.Empty;

        [Header("Room")]
        [SerializeField]
        private int maximumPlayers = 4;

        [SerializeField]
        private int minimumPlayers = 2;

        [SerializeField]
        private NetworkRoomPlayerLobby roomPlayerPrefab = null;

        [SerializeField]
        private GameObject botPrefab = null;

        [SerializeField]
        private SpawnPositionPicker spawnPositionPicker = null;

        [SerializeField]
        private Diggable.Core.Storage gemStorage = null;    
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
        #endregion

        public static event Action OnClientConnected;
        public static event Action OnClientDisconnnected;

        public int MinNumPlayers => minimumPlayers; 
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

        public Player SpawnNetworkPlayer(NetworkConnection conn)
        {
            var player = Instantiate(NetworkPlayerPrefab, spawnPositionPicker.NextSpawnPoint, Quaternion.identity);
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
            SpawnMapGenerator();  
            SpawnDiggableGenerator();            
        }

        private void SpawnDiggableGenerator()
        {
            var diggableGeneratorGO = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals(DIGGABLE_GENERATOR)));
            NetworkServer.Spawn(diggableGeneratorGO);
            DontDestroyOnLoad(diggableGeneratorGO);
            DontDestroyOnLoadObjects.Add(diggableGeneratorGO);
        }

        private void SpawnMapGenerator()
        {
            var mapGeneratorGO = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals(MAP_GENERATOR)));
            var mapGenerator = mapGeneratorGO.GetComponent<Map.Core.IMapGenerator>();
            spawnPositionPicker.CentreOffset = new Vector2(mapGenerator.MapWidth / 2, mapGenerator.MapHeight / 2);
            NetworkServer.Spawn(mapGeneratorGO);
            DontDestroyOnLoad(mapGeneratorGO);
            DontDestroyOnLoadObjects.Add(mapGeneratorGO);
        }

        public void SpawnPvPPlayers()
        {
            RoomPlayers.ToArray().ForEach(roomPlayer =>
            {
                var player = Instantiate(NetworkPlayerPrefab, spawnPositionPicker.NextSpawnPoint, Quaternion.identity);
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
            
            Players.ForEach(player => SpawnSonar(player.connectionToClient));            
            Players.ForEach(player => SpawnDiggableGeneratorCommunicator(player.connectionToClient));  
            Players.ForEach(player => GenMapRenderer(player.connectionToClient)); 
            Players.ForEach(player => SpawnStorage(player.netIdentity));

            SpawnScanWaveSpawner();
            //TODO check if all players loaded scene
            SetupGame();           
        }
        private void SpawnScanWaveSpawner()
        {
            var scanWaveSpawner = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals(SCAN_WAVE_SPAWNER)));
            NetworkServer.Spawn(scanWaveSpawner);
        } 

        private void SpawnStorage(NetworkIdentity playerId)
        {
            var storage = Instantiate(gemStorage, playerId.transform.position, Quaternion.identity);
            storage.Initialize(playerId);
            NetworkServer.Spawn(storage.gameObject);
        }

        private void SpawnSonar(NetworkConnection conn)
        {
            var sonar = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals(SONAR)));
            NetworkServer.Spawn(sonar, conn);
        }

        private void SpawnDiggableGeneratorCommunicator(NetworkConnection conn)
        {
            var digGenComm = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals(DIGGABLE_GENERATOR_COMMUNICATOR)));
            NetworkServer.Spawn(digGenComm, conn);
        }

        private void GenMapRenderer(NetworkConnection conn)
        {
            var mapRenderer = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals(MAP_RENDERER)));
            NetworkServer.Spawn(mapRenderer, conn);
        }

        private void SetupGame()
        {
            float matchTime = 120f;
            Time.timeScale = 1f;   
            gameModeManager.SetupGame();      
            Invoke(nameof(EndGame), matchTime);
        }

        public void SetupPlayerState(float matchTime)
        {
            foreach (Player player in Players)
            {
                player.Movable(true);
                player.TargetNotifyGameReady(matchTime);
            }
        }

        public void SetupBotState()
        {
            // var bot = Instantiate(botPrefab, spawnPointPicker.NextSpawnPoint, Quaternion.identity);
            var bot = Instantiate(botPrefab, spawnPositionPicker.NextSpawnPoint, Quaternion.identity);
            Bots.Add(bot.GetComponent<PlayerBot>());
            NetworkServer.Spawn(bot, Players[0].connectionToClient);
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

        private void EndGame()
        {
            Debug.Log("Player count: " + Players.Count);
            //stop game on server
            if (Players.Count <= 0) return;
            Time.timeScale = 0f;
            //if play with bot
            if (Bots.Count > 0)
            {
                Players[0].TargetNotifyEndGame(Players[0].FinalScore >= Bots[(int)0].CurrentScore);
                return;
            }
            
            Players.ForEach(player => player.Movable(false));
            List<Player> orderedPlayers = Players.OrderBy(player => -player.FinalScore).ToList<Player>();
            int highestScore = orderedPlayers[0].FinalScore;
            orderedPlayers[0].TargetNotifyEndGame(true);
            foreach (Player player in orderedPlayers.Skip(1))
            {
                if (player.FinalScore == highestScore)
                {
                    //tied
                    player.TargetNotifyEndGame(true);
                    continue;
                }
                player.TargetNotifyEndGame(false);
            }
        }

        void Update()
        {
    #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CancelInvoke();
                EndGame();
            }
    #endif
        }   
    }
}    
