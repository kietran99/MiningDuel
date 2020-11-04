using System;
using System.Linq;
using System.Collections;
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
    [SerializeField]
    private Player networkPlayerPrefab = null;

    public List<NetworkRoomPlayerLobby> roomPlayers {get;} = new List<NetworkRoomPlayerLobby>();
    public List<Player> players {get;} = new List<Player>();

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
        Debug.Log("num palyer " + numPlayers);
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
            roomPlayers.Remove(player);
            NotifyPlayersOfReadyState();
        }
        base.OnServerDisconnect(conn);
    }
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            bool isHost = roomPlayers.Count == 0;
            NetworkRoomPlayerLobby roomPlayer = Instantiate(roomPlayerPrefab);
            roomPlayer.IsHost = isHost;
            NetworkServer.AddPlayerForConnection(conn, roomPlayer.gameObject);
        }

    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in roomPlayers)
        {
            player.HandleReadyToStart(isReadyToStart());
        }
    }


    public override void OnStopServer()
    {
        roomPlayers.Clear();
    }

    public bool isReadyToStart()
    {
        if (numPlayers < minimumPlayers)    return false;
        foreach(var player in roomPlayers)
        {
            if (!player.isReady) return false;
        }
        return true;
    }

    public override void ServerChangeScene(string sceneName)
    {
        if (true)
        {
            foreach (NetworkRoomPlayerLobby roomPlayer in roomPlayers.ToArray())
            {
                Debug.Log("sapwn player");
                var player =  Instantiate(networkPlayerPrefab);
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
            if (isReadyToStart())
            {
                Debug.Log("here3");
                ServerChangeScene(gamePlayScene);
            }
        }
    }
}
