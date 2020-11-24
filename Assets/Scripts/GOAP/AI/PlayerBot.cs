using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MD.Character;
using System.Linq;
public class PlayerBot : NetworkBehaviour
{
    public float speed = 3f;
    private Vector2 minMoveBound,maxMoveBound;
    private Vector2 offset = new Vector2(.5f, .5f);
    [SerializeField]
    public int score;
    [SerializeField]
    private float viewRange = 6f;
    private Vector2 movePos = Vector2.zero;
    [SerializeField]
    private bool isMoving = false;
    [SerializeField]
    private bool isDigging = false;
    [SerializeField]
    private bool isThrowing = false;
    private bool isWandering = false;
    private IMapManager mapManager = null;
    private Rigidbody2D body;
    private BotAnimator animator;

    private DigAction digAction;
    private BotThrowAction throwAction;
    private Player player;
    [SerializeField]
    private List<GameObject> checkPoints = new List<GameObject>();
    private int checkPointIdx = 0;
    // Start is called before the first frame update
    void Awake()
    {
        GameObject.FindGameObjectsWithTag("CheckPoint").ForEach(checkPoints.Add);
        checkPoints = checkPoints.OrderBy(g => g.name).ToList();
    }
    void Start()
    {
        if(!hasAuthority) return;
        ServiceLocator.Resolve<Player>(out player);
        score =0;
        digAction = GetComponent<DigAction>();
        throwAction = GetComponent<BotThrowAction>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<BotAnimator>();
        minMoveBound = MapConstants.MAP_MIN_BOUND;
        maxMoveBound = MapConstants.MAP_MAX_BOUND;
        ServiceLocator.Resolve<IMapManager>(out mapManager);
    }

    bool digBomb = false, takeControl= false;
    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            digBomb = !digBomb;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            takeControl = !takeControl;
        }
        if (takeControl)
        {
            isMoving = false;
            return;
        }
        #endif
        if(!hasAuthority) return;
        if (isMoving || isWandering || isDigging || isThrowing) return;
        MakeDecision();
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

    private bool GetClosestDiggable(out Vector2 pos)
    {
        Vector2 sqrCenter = new Vector2(Mathf.FloorToInt(transform.position.x) +.5f, Mathf.FloorToInt(transform.position.y) +.5f);
        if (digBomb?mapManager.GetMapDataAtPos(sqrCenter).ToDiggable().IsBomb():mapManager.GetMapDataAtPos(sqrCenter).ToDiggable().IsGem()) {
            pos = sqrCenter;
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
                    if (digBomb?mapManager.GetMapDataAtPos(position).ToDiggable().IsBomb():mapManager.GetMapDataAtPos(position).ToDiggable().IsGem()){
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
        if (throwAction.IsHodlingProjectile()) return false;
        return digBomb?mapManager.GetMapDataAtPos(transform.position).ToDiggable().IsBomb():mapManager.GetMapDataAtPos(transform.position).ToDiggable().IsGem();
    }
    private bool CanThrow() => throwAction.IsHodlingProjectile();
    private IEnumerator Dig()
    {
        isDigging =true;
        while (true)
        {
            if(!(digBomb?mapManager.GetMapDataAtPos(transform.position).ToDiggable().IsBomb():mapManager.GetMapDataAtPos(transform.position).ToDiggable().IsGem()))
            {
                isDigging = false;
                yield break;
            }
            isMoving = false;
            yield return new WaitForSeconds(.5f);
            isMoving = true;
            animator.InvokeDig();
            digAction.CmdDig();
        }

    }
    
    void FixedUpdate()
    {
        if (!hasAuthority) return;
        #if UNITY_EDITOR
        if(takeControl)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                digAction.CmdDig();
                animator.InvokeDig();
            }
            var moveX = Input.GetAxisRaw("Horizontal");
            var moveY = Input.GetAxisRaw("Vertical");
            Vector2 moveDir = new Vector2(moveX,moveY);
            transform.Translate(moveDir.normalized*speed*Time.fixedDeltaTime);
            animator.SetMovementState(moveDir);
            return;
        }
        #endif
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
                if (GetClosestDiggable(out pos))
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

    public void MakeDecision()
    {
        if (CanDig())
        {
            StartCoroutine(Dig());
            return;
        }
        if(CanThrow())
        {
            StartCoroutine(ThrowBomb());
            return;
        }
        digBomb = CanSeePlayer(viewRange);
        //if see player
        if(GetClosestDiggable(out movePos))
        {
            MoveToPos();
        }
        else 
        {
            Wander();
        }
    }

    public IEnumerator ThrowBomb()
    {
        isThrowing = true;
        yield return new WaitForSeconds(Random.Range(0f,2f));
        if (throwAction.IsHodlingProjectile())       
            throwAction.ThrowProjectile();
        isThrowing = false;
    }

    public bool CanSeePlayer(float range)
    {        
        RaycastHit2D[] hits =  Physics2D.RaycastAll(transform.position,(player.transform.position - transform.position).normalized, range);
        Debug.DrawLine(transform.position,transform.position + (player.transform.position - transform.position).normalized*range, Color.red, 2f);
        if (hits.Length > 0 )
        {
            for (int i = 0; i< hits.Length; i++)
            {
                if (hits[i].collider.CompareTag(Constants.PLAYER_TAG))
                {
                    Debug.Log("found Player");
                    return true;
                }              
            }
        }
        return false;
    }


    public int GetCurrentScore() => score;
    public void DecreaseScore(int amount)
    {
        score -= amount;
        score = score<0 ?0:score;
    }
}
