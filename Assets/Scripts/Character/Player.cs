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
    bool canMove;

    [SyncVar]
    bool isReady = true;

}
