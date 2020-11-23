using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MD.Character;
public class PlayerBot : NetworkBehaviour
{
    public float speed = 3f;
    private Vector2 minMoveBound,maxMoveBound;
    private Vector2 offset = new Vector2(.5f, .5f);
    [SerializeField]
    [SyncVar]
    public int score;
    private Vector2 movePos = Vector2.zero;
    [SerializeField]
    private bool isMoving = false;
    [SerializeField]
    private bool isDigging = false;

    private bool isWandering = false;
    private IMapManager mapManager = null;
    private Rigidbody2D body;
    private BotAnimator animator;

    private DigAction digAction;

    [SerializeField]
    private List<GameObject> checkPoints = new List<GameObject>();
    private int checkPointIdx = 0;
    // Start is called before the first frame update
    void Awake()
    {
        GameObject.FindGameObjectsWithTag("CheckPoint").ForEach(checkPoints.Add);
    }
    void Start()
    {
        if(!hasAuthority) return;
        score =0;
        digAction = GetComponent<DigAction>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<BotAnimator>();
        minMoveBound = MapConstants.MAP_MIN_BOUND;
        maxMoveBound = MapConstants.MAP_MAX_BOUND;
        ServiceLocator.Resolve<IMapManager>(out mapManager);
    }

    void Update()
    {
        if(!hasAuthority) return;
        if (isMoving || isWandering || isDigging) return;
        if (CanDig())
        {
            StartCoroutine(Dig());
        }
        else if(GetClosestGemPos(out movePos))
        {
            MoveToPos();
        }
        else 
        {
            Wander();
        }
    }

    private void Wander()
    {
        isWandering = true;
        float sqrclosestDistant = Mathf.Infinity;
        float sqrDistance = 0;
        checkPointIdx = 0;
        for (int i= 0; i< checkPoints.Count; i++)
        {
            sqrDistance = Vector2.SqrMagnitude(gameObject.transform.position - checkPoints[i].transform.position);
            if (sqrDistance < sqrclosestDistant)
            {
                sqrclosestDistant =  sqrDistance;
                checkPointIdx = i;
            }
        }
        movePos = checkPoints[checkPointIdx].transform.position;
    }

    [Server]
    private bool GetClosestGemPos(out Vector2 pos)
    {
        Vector2 sqrCenter = new Vector2(Mathf.FloorToInt(transform.position.x) +.5f, Mathf.FloorToInt(transform.position.y) +.5f);
        if (mapManager.GetMapDataAtPos(sqrCenter).ToDiggable().IsGem()){
            pos = sqrCenter;
            Debug.Log("1" + mapManager.GetMapDataAtPos(sqrCenter).ToDiggable() +" is gem:"+ mapManager.GetMapDataAtPos(sqrCenter).ToDiggable().IsGem());
            return true;
        } 
        Vector2 position = Vector2.zero;
        for (int i=1;i<=3;i++)
        {
            for (int x = -i; x <=i ; x++)
            {
                for (int y = -i; y <=i; y++)
                {
                    if(x != i && x!= -i && y!=i && y!= -i) continue;
                    position = sqrCenter + new Vector2((float)x,(float)y);
                    if (mapManager.GetMapDataAtPos(position).ToDiggable().IsGem()){
                        pos = position;
                        return true;
                    }
                }
            }
        }
        pos = default;
        return false;
    }

    private void MoveToPos()
    {
        isMoving = true;
    }
    private bool CanDig()
    {
        return mapManager.GetMapDataAtPos(transform.position).ToDiggable().IsGem()? true : false;
    }
    private IEnumerator Dig()
    {
        isDigging =true;
        while (true)
        {
            if(!mapManager.GetMapDataAtPos(transform.position).ToDiggable().IsGem())
            {
                isDigging = false;
                yield break;
            }
            digAction.CmdDig();
            animator.InvokeDig();
            yield return new WaitForSeconds(1f);
        }

    }
    
    void FixedUpdate()
    {
        if (!hasAuthority) return;
        if (isMoving) {
            if (Vector2.Distance(movePos,transform.position) < .1f)
            {
                transform.position = movePos;
                isMoving = false;
                return;
            }
            Vector2 moveDir = movePos - (Vector2)transform.position;
            animator.SetMovementState(moveDir);
            transform.Translate(moveDir.normalized*speed*Time.fixedDeltaTime);
        }
        else if (isWandering)
        {
            if (Vector2.Distance(movePos,transform.position) < .1f)
            {
                transform.position = movePos;
                Vector2 pos;
                if (GetClosestGemPos(out pos))
                {
                    movePos = pos;
                    MoveToPos();
                    isWandering = false;
                    return;
                }
                checkPointIdx++;
                if (checkPointIdx > checkPoints.Count -1 ) checkPointIdx = 0;
                movePos = checkPoints[checkPointIdx].transform.position;
                return;
            }
            Vector2 moveDir = movePos - (Vector2)transform.position;
            transform.Translate(moveDir.normalized*speed*Time.fixedDeltaTime);
            animator.SetMovementState(moveDir);
        }

    }
}
