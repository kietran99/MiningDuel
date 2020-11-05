using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class NetworkManagerLobby : NetworkManager
{
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

    private readonly string NAME_PLAYER_ONLINE = "Player Online";
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

    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
    public List<Player> Players { get; } = new List<Player>();

    private readonly string NAME_CAMERA = "Main Camera";

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
        if (true)
        {
            foreach (NetworkRoomPlayerLobby roomPlayer in RoomPlayers.ToArray())
            {
                //Debug.Log("Spawn cameras");
                //var camera = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals(NAME_CAMERA)),
                //    new Vector3(0f, 0f, -10f), Quaternion.identity);
                //NetworkServer.Spawn(camera);
                
                Debug.Log("Spawn players");
                //var player =  Instantiate(networkPlayerPrefab);
                //var player = Instantiate(spawnPrefabs.Find(prefab => prefab.name.Equals("Player Online"))).GetComponent<Player>();
                var player = Instantiate(NetworkPlayerPrefab);
                player.SetPlayerName(roomPlayer.DisplayName);
                var conn = roomPlayer.netIdentity.connectionToClient;
                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, player.gameObject, true);
            }
        }

        base.ServerChangeScene(sceneName);
    }

    public void StartGame()
    {
        Debug.Log("here");
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            Debug.Log("here2");
            if (IsReadyToStart())
            {
                Debug.Log("here3");
                ServerChangeScene(gamePlayScene);
            }
        }
    }
}
