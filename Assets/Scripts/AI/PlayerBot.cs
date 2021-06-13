using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MD.Character;
using System.Linq;
using MD.Diggable.Core;

namespace MD.AI
{
    public class PlayerBot : NetworkBehaviour, IPlayer
    {
        #region SERIALIZE FIELDS
        [SerializeField]
        private string currentState = null;

        [SerializeField]
        private int score = 0;

        [SerializeField]
        public bool isDigging = false;

        [SerializeField]
        private bool canSeePlayer = false;

        [SerializeField]
        public List<GameObject> checkPoints = new List<GameObject>();

        [SerializeField]
        private MD.Diggable.Projectile.ProjectileLauncher exposedBombPrefab = null;

        [SerializeField]
        private float nearbyDetectSquareRange = 36f;
        #endregion

        #region FIELDS
        public float holdBombTime = 4f;
        public float digCoolDown = .5f;
        private Vector2 minMoveBound, maxMoveBound;
        private Camera mainCam = null;
        // [SerializeField]
        // private bool isThrowing = false;
        // private bool isWandering = false;
        // private IDiggableGenerator diggableGenerator;
        private Player target;
        private Rigidbody2D body;
        private BotAnimator animator;
        private BotDigAction digAction;
        private BotThrowAction throwAction;
        public Vector2 lastSeenPlayer = Vector2.zero;
        private BotMoveAction moveAction;
        private BotBasicAttackAction attackAction;
        private float nextDigTime = 0f;
        private FSMState FSM;
        // public int checkPointIdx = 0;
        #endregion

        public Player Target { get => target; set => target = value; }

        public bool CanSeePlayer => canSeePlayer;

        public int CurrentScore => score;

        public bool IsMoving => moveAction.IsMoving;

        public bool Throwable => throwAction.IsHoldingProjectile;


        void Awake()
        {
            GameObject.FindGameObjectsWithTag("CheckPoint").ForEach(checkPoints.Add);
            checkPoints = checkPoints.OrderBy(g => g.name).ToList();
        }

        void Start()
        {
            if(!hasAuthority) return;

            mainCam = Camera.main;
            ServiceLocator.Resolve<Player>(out target);
            score = 0;
            digAction = GetComponent<BotDigAction>();
            throwAction = GetComponent<BotThrowAction>();
            body = GetComponent<Rigidbody2D>();
            animator = GetComponent<BotAnimator>();
            moveAction = GetComponent<BotMoveAction>();
            attackAction = GetComponent<BotBasicAttackAction>();
            moveAction.SetAnimator(animator);
            minMoveBound = MapConstants.MapMinBound;
            maxMoveBound = MapConstants.MapMaxBound;
            FSM = new PB_Idle(this);
        }

        void Update()
        {
            if (moveAction.IsStuned()) return;
            CheckCanSeePlayer();
            currentState = FSM.name.ToString();
            FSM = FSM.Process();
        }

        public void ResetFMS()
        {
            FSM =  new PB_Idle(this);
        }

        public void StartMoving() => moveAction.startMoving();

        public void StopMoving() => moveAction.StopMoving();

        public bool SetMovePosition(Vector2 movePos) => moveAction.SetMovePos(movePos);

        public void StartWandering() => moveAction.StartWandering();

        public void Attack() => attackAction.Attack();

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

        public bool GetClosestDiggable(out Vector2 pos, bool digProj)
        {
            Vector2 sqrCenter = new Vector2(Mathf.FloorToInt(transform.position.x) + .5f, Mathf.FloorToInt(transform.position.y) + .5f);

            if (digProj ? IsProjectileAt(sqrCenter) : IsGemAt(sqrCenter)) 
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

                        if (digProj ? IsProjectileAt(position) : IsGemAt(position))
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
            if (throwAction.IsHoldingProjectile) return false;

            return digBomb ? IsProjectileAt(transform.position) : IsGemAt(transform.position);
        }

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

        [Server]
        public void SpawnProjectile(DiggableType projType)
        {
            var holdingProjectile = Instantiate(exposedBombPrefab, transform);
            holdingProjectile.transform.position = new Vector3(0, 1f, 0);
            holdingProjectile.SetThrower(netIdentity);
            throwAction.SetHoldingProjectile(holdingProjectile);
            NetworkServer.Spawn(holdingProjectile.gameObject);
        }

        public void ThrowProjectile()
        {
            if (throwAction.IsHoldingProjectile)       
            {
                throwAction.ThrowProjectile();
            }
        }

        public bool IsHoldingProjectile() => throwAction.IsHoldingProjectile;

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
                lastSeenPlayer = Target.transform.position;
                return;
            }
            canSeePlayer = false;
        }

        public bool IsPlayerNearby()
        {
            Vector2 distance;
            distance.x = target.transform.position.x - transform.position.x;
            distance.y = target.transform.position.y - transform.position.y;
            return (distance.sqrMagnitude  <=  nearbyDetectSquareRange);
        }

        public float GetSquarePlayerDistance()
        {
            Vector2 distance;
            distance.x = target.transform.position.x - transform.position.x;
            distance.y = target.transform.position.y - transform.position.y;
            return distance.sqrMagnitude;
        }

        public void IncreaseScore(int amount) => score += amount;

        public void DecreaseScore(int amount)
        {
            score -= amount;
            score = score < 0 ? 0 : score;
        }

        [Server]
        private bool IsProjectileAt(Vector2 pos)
        {
            var res = true;

            if (!(res = ServiceLocator.Resolve(out IDiggableGenerator diggableGenerator)))
            {
                return res;
            }

            res = diggableGenerator.IsProjectileAt(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y)).Match(err => false, isProjAt => isProjAt);

            return res;
        }

        [Server]
        private bool IsGemAt(Vector2 pos)
        {
            var res = true;

            if (!(res = ServiceLocator.Resolve(out IDiggableGenerator diggableGenerator)))
            {
                return res;
            }

            res = diggableGenerator.IsGemAt(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y)).Match(err => false, isProjAt => isProjAt);

            return res;
        }
        public NetworkIdentity GetNetworkIdentity() => netIdentity;
        public int GetUID () => GetInstanceID();  
    }
}
