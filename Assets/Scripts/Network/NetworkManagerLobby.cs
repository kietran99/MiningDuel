using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
public class NetworkManagerLobby : NetworkManager
{
    [Scene] [SerializeField]
    private string menuScene = string.Empty;
    [Header("Room")]
    [SerializeField]
    private int maximumPlayers = 4;
    [SerializeField]
    private int minimumPlayers = 2;
    [SerializeField]
    private NetworkRoomPlayerLobby roomPlayerPrefab = null;

    public List<NetworkRoomPlayerLobby> roomPlayers {get;} = new List<NetworkRoomPlayerLobby>();

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
        if (SceneManager.GetActiveScene().name != menuScene)
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
        if (numPlayers < maximumPlayers)    return false;
        foreach(var player in roomPlayers)
        {
            if (!player.isReady) return false;
        }
        return true;
    }
}
