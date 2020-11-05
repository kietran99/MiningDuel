using UnityEngine;
using Mirror;
public class Player : NetworkBehaviour
{
    static public Player LocalPlayer = null; 
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

    public static event System.Action<GameObject> OnPlayerSpawn;

    public override void OnStartClient()
    {
        DontDestroyOnLoad(this);
        Room.Players.Add(this);
    }

    public override void OnStartLocalPlayer()
    {
        OnPlayerSpawn?.Invoke(gameObject);
    }

    public override void OnStopClient()
    {
        Room.Players.Remove(this);
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        LocalPlayer = this;
    }

    [Server]
    public void SetPlayerName(string name)
    {
        playerName = name;
    }

}
