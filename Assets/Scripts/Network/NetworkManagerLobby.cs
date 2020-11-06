using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

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
    }

    public bool IsReadyToStart()
    {
        if (numPlayers < minimumPlayers)    return false;
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

            base.OnServerSceneChanged(sceneName);
            Debug.Log("Spawn Map Manager");
            mapManager = Instantiate(MapManagerPrefab);
            DontDestroyOnLoad(mapManager);
            NetworkServer.Spawn(mapManager.gameObject);

            foreach (NetworkRoomPlayerLobby roomPlayer in RoomPlayers.ToArray())
            {                
                Debug.Log("Spawn players");               
                var player = Instantiate(NetworkPlayerPrefab, spawnPointPicker.NextSpawnPoint.position, Quaternion.identity);
                player.SetPlayerName(roomPlayer.DisplayName);
                var conn = roomPlayer.netIdentity.connectionToClient;
                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, player.gameObject, true);
                player.TargetRegisterIMapManager(mapManager.netIdentity);
            }
        }
        base.ServerChangeScene(sceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        //check ingame scene
        mapManager.GenerateMap();
    }

    public void StartGame()
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
