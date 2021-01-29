using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MD.Character;
using System.Linq;

namespace MD.AI
{
    public class PlayerBot : NetworkBehaviour
    {
        [SerializeField]
        private string currentState = null;
        public float holdBombTime = 4f;
        public float digCoolDown = .5f;
        private Vector2 minMoveBound,maxMoveBound;
        private Vector2 offset = new Vector2(.5f, .5f);
        [SerializeField]
        public int score;

        [SerializeField]
        public bool isDigging = false;

        [SerializeField]
        private bool canSeePlayer = false;

        private Camera mainCam = null;
        // [SerializeField]
        // private bool isThrowing = false;
        // private bool isWandering = false;
        private IMapManager mapManager = null;
        private Rigidbody2D body;
        private BotAnimator animator;

        private DigAction digAction;
        private BotThrowAction throwAction;
        public Player player;

        public Vector2 lastSeenPlayer = Vector2.zero;
        [SerializeField]
        public List<GameObject> checkPoints = new List<GameObject>();
        private BotMoveAction moveAction;

        private float nextDigTime = 0f;
        private FSMState FMS;
        // public int checkPointIdx = 0;
        // Start is called before the first frame update
        void Awake()
        {
            GameObject.FindGameObjectsWithTag("CheckPoint").ForEach(checkPoints.Add);
            checkPoints = checkPoints.OrderBy(g => g.name).ToList();
        }
        void Start()
        {
            if(!hasAuthority) return;
            mainCam = Camera.main;
            ServiceLocator.Resolve<Player>(out player);
            score = 0;
            digAction = GetComponent<DigAction>();
            throwAction = GetComponent<BotThrowAction>();
            body = GetComponent<Rigidbody2D>();
            animator = GetComponent<BotAnimator>();
            moveAction = GetComponent<BotMoveAction>();
            moveAction.SetAnimator(animator);
            minMoveBound = MapConstants.MAP_MIN_BOUND;
            maxMoveBound = MapConstants.MAP_MAX_BOUND;
            ServiceLocator.Resolve<IMapManager>(out mapManager);
            FMS = new PB_Idle(this);
        }

        void Update()
        {
            CheckCanSeePlayer();
            currentState = FMS.name.ToString();
            FMS = FMS.Process();
        }

        public bool IsMoving() => moveAction.IsMoving();
        public void StartMoving() => moveAction.startMoving();

        public void SetMovePosition(Vector2 movePos) => moveAction.SetMovePos(movePos);



        // bool digBomb = false, takeControl = false;

        // void Update()
        // {
        //     #if UNITY_EDITOR
        //     if (Input.GetKeyDown(KeyCode.R))
        //     {
        //         digBomb = !digBomb;
        //     }
        //     if (Input.GetKeyDown(KeyCode.E))
        //     {
        //         takeControl = !takeControl;
        //     }
        //     if (takeControl)
        //     {
        //         isMoving = false;
        //         return;
        //     }
        //     #endif

        //     if(!hasAuthority) return;

        //     if (isMoving || isWandering || isDigging || isThrowing) return;

        //     MakeDecision();
        // }

        // public void MakeDecision()
        // {
        //     if (CanDig())
        //     {
        //         StartCoroutine(Dig());
        //         return;
        //     }

        //     if(CanThrow())
        //     {
        //         StartCoroutine(ThrowBomb());
        //         return;
        //     }

        //     digBomb = CanSeePlayer(viewRange);
        //     //if see player
        //     if(GetClosestDiggable(out movePos))
        //     {
        //         MoveToPos();
        //     }
        //     else 
        //     {
        //         Wander();
        //     }
        // }

        // private void Wander()
        // {
        //     float sqrclosestDistant = Mathf.Infinity;
        //     float sqrDistance = 0;
        //     checkPointIdx = 0;

        //     for (int i = 0; i < checkPoints.Count; i++)
        //     {
        //         sqrDistance = Vector2.SqrMagnitude(gameObject.transform.position - checkPoints[i].transform.position);
        //         if (sqrDistance < sqrclosestDistant)
        //         {
        //             sqrclosestDistant = sqrDistance;
        //             checkPointIdx = i;
        //         }
        //     }

        //     movePos = checkPoints[checkPointIdx].transform.position;
        // }
        

        public int GetClosestWayPointIndex()
        {
            float sqrclosestDistant = Mathf.Infinity;
            float sqrDistance = 0;
            int currentIndex = 0;
            for (int i = 0; i < checkPoints.Count; i++)
            {
                sqrDistance = Vector2.SqrMagnitude(transform.position - checkPoints[i].transform.position);
                if (sqrDistance < sqrclosestDistant)
                {
                    sqrclosestDistant = sqrDistance;
                    currentIndex = i;
                }
            }
            return currentIndex;
        }

        public bool GetClosestDiggable(out Vector2 pos,bool digBomb)
        {
            Vector2 sqrCenter = new Vector2(Mathf.FloorToInt(transform.position.x) + .5f, Mathf.FloorToInt(transform.position.y) + .5f);

            if (digBomb ? mapManager.IsProjectileAt(sqrCenter) : mapManager.IsGemAt(sqrCenter)) 
            {
                pos = sqrCenter;
                return true;
            } 

            Vector2 position = Vector2.zero;

            for (int i = 1; i <= 3; i++)
            {
                for (int x = -i; x <= i; x++)
                {
                    for (int y = -i; y <= i; y++)
                    {
                        if (x != i && x != -i && y != i && y != -i) continue;

                        position = sqrCenter + new Vector2((float)x, (float)y);

                        if (digBomb ? mapManager.IsProjectileAt(position) : mapManager.IsGemAt(position))
                        {
                            pos = position;
                            return true;
                        }
                    }
                }
            }

            pos = default;
            return false;
        }


        // private void MoveToPos()
        // {
        //     isMoving = true;
        // }

        public bool CanDig(bool digBomb)
        {
            if (throwAction.IsHoldingProjectile()) return false;

            return digBomb ? mapManager.IsProjectileAt(transform.position) : mapManager.IsGemAt(transform.position);
        }

        public bool CanThrow() => throwAction.IsHoldingProjectile();

        // private IEnumerator Dig(bool digBomb)
        // {

        //     while (true)
        //     {
        //         if(!(digBomb? mapManager.IsProjectileAt(transform.position) : mapManager.IsGemAt(transform.position)))
        //         {
        //             yield break;
        //         }

        //         isMoving = false;
        //         yield return new WaitForSeconds(.5f);

        //         isMoving = true;
        //         animator.InvokeDig();
        //         digAction.CmdDig();
        //     }

        // }
        public void Dig()
        {
            // StartCoroutine(CoroutineDig());
            if (Time.time < nextDigTime) return;
            isDigging = true;
            nextDigTime = Time.time + digCoolDown;
            EventSystems.EventManager.Instance.TriggerEvent(new BotDigInvokeData());
        }
        // private IEnumerator CoroutineDig()
        // {
        //     isDigging = true;
        //     yield return new WaitForSeconds(.5f);
        //     animator.InvokeDig();
        //     digAction.CmdDig();
        //     isDigging = false;
        // }

        // void FixedUpdate()
        // {
        //     if (!hasAuthority) return;
        //     if (isMoving) 
        //     {
        //         if (Vector2.Distance(movePos,transform.position) < .1f)
        //         {
        //             transform.position = movePos;
        //             isMoving = false;
        //             return;
        //         }

        //         Vector2 moveDir = movePos - (Vector2)transform.position;
        //         animator.SetMovementState(moveDir);
        //         transform.Translate(moveDir.normalized*speed*Time.fixedDeltaTime);
        //     }
            // else if (isWandering)
            // {
            //     if (Vector2.Distance(movePos,transform.position) < .1f)
            //     {
            //         transform.position = movePos;
            //         Vector2 pos;
            //         if (GetClosestDiggable(out pos))
            //         {
            //             movePos = pos;
            //             MoveToPos();
            //             isWandering = false;
            //             return;
            //         }

            //         checkPointIdx++;
            //         if (checkPointIdx > checkPoints.Count -1) checkPointIdx = 0;
            //         movePos = checkPoints[checkPointIdx].transform.position;
            //         return;
            //     }

            //     Vector2 moveDir = movePos - (Vector2)transform.position;
            //     transform.Translate(moveDir.normalized * speed * Time.fixedDeltaTime);
            //     animator.SetMovementState(moveDir);
            // }
        // }

        public void ThrowBomb()
        {
            if (throwAction.IsHoldingProjectile())       
                throwAction.ThrowProjectile();
        }

        private void CheckCanSeePlayer()
        {
            // RaycastHit2D[] hits =  Physics2D.RaycastAll(transform.position,(player.transform.position - transform.position).normalized, viewRange);
            // Debug.DrawLine(transform.position,transform.position + (player.transform.position - transform.position).normalized*viewRange, Color.red, 2f);
            
            // if (hits.Length > 0 )
            // {
            //     for (int i = 0; i < hits.Length; i++)
            //     {
            //         if (hits[i].collider.CompareTag(Constants.PLAYER_TAG))
            //         {
            //             canSeePlayer = true;
            //             Debug.Log("found player");
            //             lastSeenPlayer = hits[i].transform.position;
            //             return;
            //         }              
            //     }
            // }
            Vector2 position = Vector2.zero;
            position = mainCam.WorldToViewportPoint(transform.position);       
            if (position.x >= 0.1 && position.x <= .9 && position.y >= .1 && position.y <=.9)
            {
                canSeePlayer = true;
                lastSeenPlayer= player.transform.position;
                return;
            }
            canSeePlayer = false;
        }

        public bool CanSeePlayer()
        {        
            return canSeePlayer;
        }

        public int GetCurrentScore() => score;

        public void DecreaseScore(int amount)
        {
            score -= amount;
            score = score < 0 ? 0 : score;
        }
    }
}
