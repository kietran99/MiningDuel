using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MD.Character;
using UnityEngine.UI;
[RequireComponent(typeof(Collider2D))]
public class Storage : NetworkBehaviour
{

    private const float TIMESCHECK = 20;
    [SerializeField]
    private float storeTime = 2f;
    [SerializeField]    
    private float checkTime;
    private NetworkIdentity ownerID;
    private bool isInside;

    [SerializeField]
    private GameObject ProcessBar;
    [SerializeField]
    private Image ProcessBarImage;

    public override void OnStartServer()
    {
        base.OnStartServer();
        checkTime = storeTime/TIMESCHECK;
    }

    public void Initialize(NetworkIdentity ownerID)
    {
        this.ownerID = ownerID;
    }

    [ServerCallback]
    void OnTriggerEnter2D(Collider2D collide)
    {
        if (!collide.CompareTag("Player")) return;
        Player player = collide.gameObject.GetComponent<Player>();
        if ( player == null || player.netIdentity != ownerID) return;
        isInside = true;
        TargetShowProcessBar(ownerID.connectionToClient);
        StartCoroutine(nameof(storingScore));
    }
    [ServerCallback]
    void OnTriggerExit2D(Collider2D collide)
    {
        if (!collide.CompareTag("Player")) return;
        Player player = collide.gameObject.GetComponent<Player>();
        if ( player == null || player.netIdentity != ownerID) return;
        TargetHideProcessBar(ownerID.connectionToClient);
        isInside = false;
    }

    IEnumerator storingScore()
    {
        var waitTime = new WaitForSeconds(checkTime);
        for (int i = 1; i<=TIMESCHECK; i++)
        {
            yield return waitTime;
            if (!isInside) yield break;
            //play animation in rpc
            TargetShowProcess(ownerID.connectionToClient, (float)i /TIMESCHECK);
        }
        //storing finished, fire an event
        EventSystems.EventManager.Instance.TriggerEvent(new StoreFinishedData(ownerID));
    }
    [TargetRpc]
    private void TargetShowProcess(NetworkConnection conn, float amount)
    {
        if (!ProcessBar.activeInHierarchy) ProcessBar.SetActive(true);
        ProcessBarImage.fillAmount = amount;
    }

    [TargetRpc]
    private void TargetHideProcessBar(NetworkConnection conn)
    {
        ProcessBar.SetActive(false);
    }

    [TargetRpc]
    private void TargetShowProcessBar(NetworkConnection conn)
    {
        ProcessBarImage.fillAmount = 0;
        ProcessBar.SetActive(true);
    }
}
