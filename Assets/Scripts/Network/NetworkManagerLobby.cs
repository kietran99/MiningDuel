using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using MD.Character;

namespace MD.UI
{
    public class NetworkManagerLobby : NetworkManager
    {
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
        private SpawnPointPicker spawnPointPicker = null;    
        #endregion

        private readonly string NAME_PLAYER_ONLINE = "Player Online";
        private readonly string MAP_MANAGER = "Map Manager";

        public List<GameObject> DontDestroyOnLoadObjects = new List<GameObject>();

        private Player networkPlayerPrefab = null;

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

        private MapManager mapManagerPrefab = null;
        private MapManager MapManagerPrefab
        {
            set => mapManagerPrefab = value;
            get
            {
                if (mapManagerPrefab != null) return mapManagerPrefab;
                mapManagerPrefab = spawnPrefabs.Find(prefab => prefab.name.Equals(MAP_MANAGER)).GetComponent<MapManager>();
                return mapManagerPrefab;
            }
        }

        public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
        public List<Player> Players { get; } = new List<Player>();

        public List<PlayerBot> Bots {get;} = new List<PlayerBot>();

        private MapManager mapManager;

        public static event Action OnClientConnected;
        public static event Action OnClientDisconnnected;

        public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

        public override void OnStartClient()
        {
            var spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");
            foreach (var prefab in spawnPrefabs)
            {
                ClientScene.RegisterPrefab(prefab);
            }

            //fadeStartCompleteEvent.OnEventRaise += LoadGameScene;
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            OnClientConnected?.Invoke();
            
        }

        public void CleanObjectsWhenDisconnect()
        {
            Debug.Log("clean object when disconnect");
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
            //fadeStartCompleteEvent.OnEventRaise -= LoadGameScene;
            OnClientDisconnnected?.Invoke();
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            Debug.Log("Num players: " + numPlayers);
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
                    NotifyPlayersOfReadyState();
                }
                else if (SceneManager.GetActiveScene().path == gamePlayScene)
                {
                    var player = conn.identity.GetComponent<Player>();
                    Players.Remove(player);
                }
            }
            base.OnServerDisconnect(conn);
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {        
            if (SceneManager.GetActiveScene().path == menuScene)
            {
                bool isHost = RoomPlayers.Count == 0;
                NetworkRoomPlayerLobby roomPlayer = Instantiate(roomPlayerPrefab);
                roomPlayer.IsHost = isHost;
                NetworkServer.AddPlayerForConnection(conn, roomPlayer.gameObject);
            }

        }

        public void NotifyPlayersOfReadyState()
        {
            foreach (var player in RoomPlayers)
            {
                player.HandleReadyToStart(IsReadyToStart());
            }
        }

        public override void OnStopServer()
        {
            //Debug.Log("on stop server");
            Time.timeScale = 1f;
            ServerChangeScene(menuScene);
            CleanObjectsWhenDisconnect();
            base.OnStopServer();
        }

        public bool IsReadyToStart()
        {
            if (numPlayers < minimumPlayers) return false;

            foreach(var player in RoomPlayers)
            {
                if (!player.isReady) return false;
            }

            return true;
        }

        public override void ServerChangeScene(string sceneName)
        {
            if (SceneManager.GetActiveScene().path.Equals(menuScene))
            {
                spawnPointPicker.Reset();     
                SpawnMapManager();
            }

            base.ServerChangeScene(sceneName);
        }

        private void SpawnMapManager()
        {        
            //Debug.Log("Spawn Map Manager");
            mapManager = Instantiate(MapManagerPrefab);
            // ServiceLocator.Register<IMapManager>(mapManager.GetComponent<IMapManager>());
            NetworkServer.Spawn(mapManager.gameObject);
            RoomPlayers.ToArray().ForEach(SpawnNetworkPlayer);
            DontDestroyOnLoad(mapManager);
            DontDestroyOnLoadObjects.Add(mapManager.gameObject); 
        }

        private void SpawnNetworkPlayer(NetworkRoomPlayerLobby roomPlayer)
        {
            //Debug.Log("Spawn a player");          
            //var player = Instantiate(NetworkPlayerPrefab);     
            var player = Instantiate(NetworkPlayerPrefab, spawnPointPicker.NextSpawnPoint.position, Quaternion.identity);
            player.SetPlayerName(roomPlayer.DisplayName);
            var conn = roomPlayer.netIdentity.connectionToClient;
            NetworkServer.Destroy(conn.identity.gameObject);
            NetworkServer.ReplacePlayerForConnection(conn, player.gameObject, true);
            // Players.Add(player);
            player.TargetRegisterIMapManager(mapManager.netIdentity);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().path == gamePlayScene)
            {
                mapManager.GenerateMap(); 
                //TODO check if all players loaded scene
                StartGame();
            }
            // if (SceneManager.GetActiveScene().path == menuScene)
            // {
            //     StopServer();
            // }
        }

        private void StartGame()
        {
            Time.timeScale = 1f;
            float matchTime = 120f;
            foreach(Player player in Players)
            {
                player.SetCanMove(true);
                player.TargetNotifyGameReady(matchTime);
            }

            if (Players.Count == 1)
            {
                var bot = Instantiate(botPrefab);
                Bots.Add(bot.GetComponent<PlayerBot>());
                NetworkServer.Spawn(bot, Players[0].connectionToClient);
            }

            Invoke(nameof(EndGame), matchTime);
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

        private void EndGame()
        {
            Debug.Log("Player count "+ Players.Count);
            //stop game in server
            if (Players.Count <=0) return;
            Time.timeScale = 0f;
            //if play with bot
            if (Bots.Count > 0)
            {
                Players[0].TargetNotifyEndGame(Players[0].CurrentScore >= Bots[0].score);
                return;
            }
            Players.ForEach(player => player.SetCanMove(false));
            List<Player> orderedPlayers = Players.OrderBy(player => -player.CurrentScore).ToList<Player>();
            int highestScore = orderedPlayers[0].CurrentScore;
            orderedPlayers[0].TargetNotifyEndGame(true);
            foreach (Player player in orderedPlayers.Skip(1))
            {
                if (player.CurrentScore == highestScore)
                {
                    //tied
                    player.TargetNotifyEndGame(true);
                    continue;
                }
                player.TargetNotifyEndGame(false);
            }
        }
        public void StartLobby()
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
    }
}    
