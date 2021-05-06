using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MD.Character;
public class LinkedTrap : NetworkBehaviour, IDamagable
{
    [SerializeField]
    float slowDownTime = 1.5f;
    [SerializeField]
    float explosionTime = .2f;
    [SerializeField]
    List<LinkedTrap> linkedTrapsList;

    [SerializeField]
    private NetworkIdentity owner ;

    [SerializeField]
    private TrapExplodeRangeControl rangeControl;

    [SerializeField]
    GameObject WirePrefab;

    [SerializeField]
    private float WireLenghPadding = .8f;

    [SerializeField]
    private ContactFilter2D explodeFilter;

    private Vector2 HALF_CELL_OFFSET = Vector2.one/2f;
    private bool isExploding;

    private Dictionary<LinkedTrap,GameObject> WireDict;
    private Vector3 BaseWireScale;

    private float MinWireLength = .3f;



    void Awake()
    {
        linkedTrapsList = new List<LinkedTrap>();
        isExploding = false;
        WireDict = new Dictionary<LinkedTrap, GameObject>();
        BaseWireScale = WirePrefab.transform.localScale;
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

    private void OnDisable()
    {
        RemoveWire();
    }

    [Command]
    public void CmdRequestDetonate()
    {
        Detonate();
    }

    [Server]
    public void Detonate()
    {
        if (isExploding) return;
        isExploding = true;
        StartCoroutine( Explode());
    }

    [Server]
    public void SpreadExplode(LinkedTrap from)
    {
        int index = linkedTrapsList.IndexOf(from);
        if (index != -1)
        {
            linkedTrapsList.RemoveAt(index);
            // RpcRemoveWire(from);
            Detonate();
        }
        else
        {
            Debug.LogError("explode triggered from out of range trap from " + from.transform.position + " this" + transform.position );
        }
        Detonate();
    }

    [Server]
    private IEnumerator Explode()
    {
        //animate
        Debug.Log("explode");
        
        yield return new WaitForSeconds(explosionTime);

        Collider2D collider = rangeControl.GetComponent<Collider2D>();
        if (collider != null)
        {
            List<Collider2D> results = new List<Collider2D>();
            collider.OverlapCollider(explodeFilter, results);
            for (int i=0; i< results.Count; i++)
            {
                if (results[i].CompareTag(Constants.PLAYER_TAG))
                {
                    IPlayer player = results[i].GetComponent<IPlayer>();
                    if (player.GetNetworkIdentity().Equals(owner)) continue;
                    MD.Diggable.Projectile.IExplodable explodable = results[i].GetComponent<MD.Diggable.Projectile.IExplodable>();
                    if (explodable != null)
                    {
                        explodable.HandleTrapExplode(slowDownTime);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("cant find collider of range control");
        }
        for (int i=0; i<linkedTrapsList.Count; i++)
        {
            if (linkedTrapsList[i].gameObject == null) 
            {
                Debug.Break();
            }
            linkedTrapsList[i].SpreadExplode(this);
        }
        NetworkServer.Destroy(gameObject);
    }
    
    [ClientRpc]
    public void RpcAssignOwnerAndLinkTraps(NetworkIdentity owner)
    {
        this.owner = owner;
        rangeControl.LinkNearbyTraps();
    }

    public NetworkIdentity GetOwner() => owner;

    public void RegistLinkedTrap(LinkedTrap trap, bool createWire)
    {
        linkedTrapsList.Add(trap);
        if (createWire)
        {
            GameObject wire = CreateWire(this.transform.position , trap.transform.position );
            WireDict.Add(trap, wire);
        }
    }

    private GameObject CreateWire(Vector2 from, Vector2 to)
    {
        Vector2 centeredFrom = from + HALF_CELL_OFFSET;
        Vector2 centeredTo = to + HALF_CELL_OFFSET;

        Debug.DrawLine(centeredFrom,centeredTo,Color.red,100f);
        float length = Vector2.Distance(centeredFrom,centeredTo)- WireLenghPadding;
        length = Mathf.Max(MinWireLength,length);
        float angle = Vector2.SignedAngle(Vector2.right, centeredTo-centeredFrom);

        GameObject wire = Instantiate(WirePrefab,Vector2.Lerp(centeredFrom,centeredTo,.5f),Quaternion.identity);
        Vector3 scale = new Vector3(BaseWireScale.x*length,BaseWireScale.y,BaseWireScale.z);
        wire.transform.localScale = scale;
        wire.transform.Rotate(Vector3.forward*angle,Space.Self);

        if (hasAuthority)
        {
            wire.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
        }

        return wire;
    }

    private void RemoveWire()
    {   
        foreach ( KeyValuePair<LinkedTrap, GameObject> entry in WireDict)
        {
            Destroy(entry.Value);
        }
    }

    [Server]
    public void TakeDamage(NetworkIdentity source, int damage)
    {
        if (source == owner)
        {
            Detonate();
        }
    }
}
