using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using MD.Character;

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
    private SpawnPointPicker spawnPointPicker = null;
    #endregion

    private readonly string NAME_PLAYER_ONLINE = "Player Online";
    private readonly string MAP_MANAGER = "Map Manager";

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
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
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
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();
            RoomPlayers.Remove(player);
            NotifyPlayersOfReadyState();
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
        RoomPlayers.Clear();
        Players.Clear();
        NetworkServer.DisconnectAll();
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
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            spawnPointPicker.Reset();     
            SpawnMapManager();
        }
        base.ServerChangeScene(sceneName);
    }

    private void SpawnMapManager()
    {        
        Debug.Log("Spawn Map Manager");
        mapManager = Instantiate(MapManagerPrefab);
        // ServiceLocator.Register<IMapManager>(mapManager.GetComponent<IMapManager>());
        NetworkServer.Spawn(mapManager.gameObject);
        DontDestroyOnLoad(mapManager);
        RoomPlayers.ToArray().ForEach(SpawnNetworkPlayer);   
    }

    private void SpawnNetworkPlayer(NetworkRoomPlayerLobby roomPlayer)
    {
        Debug.Log("Spawn a player");          
        //var player = Instantiate(NetworkPlayerPrefab);     
        var player = Instantiate(NetworkPlayerPrefab, spawnPointPicker.NextSpawnPoint.position, Quaternion.identity);
        player.SetPlayerName(roomPlayer.DisplayName);
        var conn = roomPlayer.netIdentity.connectionToClient;
        NetworkServer.Destroy(conn.identity.gameObject);
        NetworkServer.ReplacePlayerForConnection(conn, player.gameObject, true);
        Players.Add(player);
        player.TargetRegisterIMapManager(mapManager.netIdentity);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().path == gamePlayScene)
        {
            mapManager.GenerateMap(); 
            //TODO: check if all players loaded scene
            StartGame();
        }
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            StopServer();
        }
    }

    private void StartGame()
    {
        float matchTime = 2f;
        foreach(Player player in Players)
        {
            player.SetCanMove(true);
            player.TargetNotifyGameReady(matchTime);
        }
        Invoke(nameof(EndGame),matchTime);
    }

    private void EndGame()
    {
        //stop game in server
        Time.timeScale = 0f;
        Players.ForEach(player => player.SetCanMove(false));

        List<Player> orderedPlayers = Players.OrderBy(player => -player.GetCurrentScore()).ToList<Player>();
        int highestScore = orderedPlayers[0].GetCurrentScore();
        orderedPlayers[0].TargetNotifyEndGame(true);
        foreach (Player player in orderedPlayers.Skip(1))
        {
            if (player.GetCurrentScore() == highestScore)
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
        //Debug.Log("here");
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            //Debug.Log("here2");
            if (IsReadyToStart())
            {
                //Debug.Log("here3");
                ServerChangeScene(gamePlayScene);
            }
        }
    }
}
