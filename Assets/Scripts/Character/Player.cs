using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{    
    [Header("Game Stats")]
    [SyncVar]
    private int score;

    [SyncVar]
    private string playerName;
    
    [SyncVar]
    public bool canMove = true;

    [SyncVar]
    private bool isReady = true;

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) return room;
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(this);
        Room.Players.Add(this);
    }
    
    public override void OnStopClient()
    {
        Room.Players.Remove(this);
    }

    public override void OnStartAuthority()
    {
        ServiceLocator.Register(this);
    }

    [Server]
    public void SetPlayerName(string name)
    {
        playerName = name;
    }

}
