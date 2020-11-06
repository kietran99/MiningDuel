using UnityEngine;

public class DropObtain : MonoBehaviour
{
    // [SerializeField]
    // private int value = 1;

    [SerializeField]
    private bool canObtain;

    [SerializeField]
    private float obtainWaitTime = 3f;
    void Start()
    {
        canObtain = false;
        Invoke("EnableObtain",obtainWaitTime);
    }
    
    private void EnableObtain()
    {
        canObtain = true;
        transform.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(Constants.PLAYER_TAG) || !canObtain) return;
        IScoreManager scoreManager;
        bool exist = ServiceLocator.Resolve<IScoreManager>(out scoreManager);
        // if (exist) scoreManager.IncreaseScore(value);
        Destroy(gameObject);
    }
}
