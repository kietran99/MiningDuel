using UnityEngine;

/// <summary>
/// Attach this script to only 1 GameObject in every scene to load all services
/// </summary>
/// <remarks>
/// Lazy-instantiated singleton
/// </remarks>
public class Loader : MonoBehaviour
{
    #region SINGLETON
    //private static Loader instance;

    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //    else if (instance != this)
    //    {
    //        Destroy(gameObject);
    //    }
    //}
    #endregion

    [SerializeField]
    private GameObject mapManager = null;

    private void Start()
    {
        ServiceLocator.Register<IMapManager>(Instantiate(mapManager).GetComponent<IMapManager>());
    }

    private void OnDestroy()
    {
        ServiceLocator.Reset();
        //instance = null;
    }
}
