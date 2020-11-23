using UnityEngine;
using Mirror;

public class DropObtain : NetworkBehaviour
{
    [SerializeField]
    private int value = 1;

    [SerializeField]
    private bool canObtain;

    [SerializeField]
    private float obtainWaitTime = 3f;

    public override void OnStartServer()
    {
        canObtain = false;
        Invoke("EnableObtain", obtainWaitTime);
    }

    [Server]
    private void EnableObtain()
    {
        canObtain = true;
        transform.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        RpcChangeColor();
    }
    
    [ClientRpc]
    private void RpcChangeColor()
    {
        transform.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }

    [ServerCallback]
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(Constants.PLAYER_TAG) || !canObtain) return;

        other.GetComponent<MD.Character.Player>().IncreaseScore(value);
        Destroy(gameObject);
    }
}
