using UnityEngine;
using Mirror;
public class Player : NetworkBehaviour
{
    // #region SINGLETON
    // public static Player Instance
    // {
    //     get
    //     {
    //         if (instance != null) return instance;

    //         instance = FindObjectOfType<Player>();

    //         if (instance == null)
    //         {
    //             instance = new GameObject("Player").AddComponent<Player>();
    //         }

    //         return instance;
    //     }
    // }

    // private static Player instance;

    // private void Awake()
    // {
    //     if (instance != null)
    //     {
    //         Destroy(gameObject);
    //     }
    // }
    // #endregion 
    [Header("Game Stats")]
    [SyncVar]
    int score;

    [SyncVar]
    string playerName;
    
    [SyncVar]
    bool canMove = false;

    [SyncVar]
    bool isReady = true;

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
        Room.players.Add(this);
    }

    public override void OnStopClient()
    {
        Room.players.Remove(this);
    }



    [Server]
    public void SetPlayerName(string name)
    {
        playerName = name;
    }

}
