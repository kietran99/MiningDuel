using UnityEngine;
using Mirror;
/// <summary>
/// Attach this script to only 1 GameObject in every scene to load all services
/// </summary>
/// <remarks>
/// Lazy-instantiated singleton
/// </remarks>
public class Loader : MonoBehaviour
{
    // #region SINGLETON
    // //private static Loader instance;

    // //private void Awake()
    // //{
    // //    if (instance == null)
    // //    {
    // //        instance = this;
    // //    }
    // //    else if (instance != this)
    // //    {
    // //        Destroy(gameObject);
    // //    }
    // //}
    // #endregion

    [SerializeField]
    private GameObject networkManager = null;

    private void Awake()
    {
        if (NetworkManager.singleton == null)
            Instantiate(networkManager);
    }

    // private void OnDestroy()
    // {
    //     ServiceLocator.Reset();
    //     //instance = null;
    // }
}
