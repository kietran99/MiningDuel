using UnityEngine;

public class Player : MonoBehaviour
{
    #region SINGLETON
    public static Player Instance
    {
        get
        {
            if (instance != null) return instance;

            instance = FindObjectOfType<Player>();

            if (instance == null)
            {
                instance = new GameObject("Player").AddComponent<Player>();
            }

            return instance;
        }
    }

    private static Player instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
    }
    #endregion 
}
