using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LinkedTrap : NetworkBehaviour
{
    [SerializeField]
    List<LinkedTrap> linkedTrapsList;
    private bool isExploding;
    [SerializeField]
    private NetworkIdentity owner ;

    [SerializeField]
    private TrapExplodeRangeControl rangeControl;

    void Start()
    {
        linkedTrapsList = new List<LinkedTrap>();
        isExploding = false;
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        for (int i=0; i< spriteRenderers.Length; i++)
        {
            spriteRenderers[i].maskInteraction = SpriteMaskInteraction.None;
        }
    }

    [ClientRpc]
    public void RpcAssignOwnerAndLinkTraps(NetworkIdentity owner)
    {
        this.owner = owner;
        Debug.Log("assign authority + " + owner);
        Debug.Log("after assign authority + " + this.owner);
        rangeControl.LinkNearbyTraps();
    }

    public NetworkIdentity GetOwner() => owner;

    public void RegistLinkedTrap(LinkedTrap trap)
    {
        linkedTrapsList.Add(trap);
    }

    public void Detonate()
    {
        if (isExploding) return;
        isExploding = true;
        Explode();
    }

    public void SpreadExplode(LinkedTrap from)
    {
        int index = linkedTrapsList.IndexOf(from);
        if (index != -1)
        {
            linkedTrapsList.RemoveAt(index);
            Detonate();
        }
        else
        {
            Debug.LogError("explode triggered from out of range trap from " + from.transform.position + " this" + transform.position );
        }
    }

    private void Explode()
    {
        //animate
        Debug.Log("explode");
        for (int i=0; i<linkedTrapsList.Count; i++)
        {
            if (linkedTrapsList[i].gameObject == null) 
            {
                Debug.Break();
            }
            linkedTrapsList[i].SpreadExplode(this);
        }
        Destroy(gameObject);
    }
}
