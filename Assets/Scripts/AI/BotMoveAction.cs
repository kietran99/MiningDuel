using UnityEngine;
using Mirror;
using PathFinding;
using System.Collections.Generic;
using System.Collections;
using MD.Character;

namespace MD.AI
{
    [RequireComponent(typeof(PlayerBot))]
    public class BotMoveAction : NetworkBehaviour
    {
        private float DASH_MULTIPLIER = 1000f;
        public float speed = 3f;
        // private int resCount = 0;
        // private bool collideLeft = false;
        // private float collideLeftDistance = 0f;
        // private bool collideRigth = false;

        // private float collideRightDistance = 0f;
        // private bool collideAhead = false;
        [SerializeField]
        MD.VisualEffects.SlowEffect slowEffect = null;
        
        [SerializeField]
        GameObject stunnedIcon = null;

        [SerializeField]
        private float SlowDownPercentage = .8f;
        [SerializeField]
        private int length;
        [SerializeField]
        private int currentNode;

        [SerializeField]
        private Vector2 currentGoal;

        private List<PathFinding.Node> path;

        [SerializeField]
        private bool isMoving = false;

        [SerializeField]
        private bool hasPath = false;

        [SerializeField]
        private float knockbackForce = 2f;


        [SerializeField]
        private float counterSuccessDashDistance = .2f;

        private int slowDownCount = 0;

        private BotAnimator animator;

        private AStar aStar;

        private MD.Map.Core.IMapGenerator mapGenerator;

        private int mapWidth, mapHeight;

        private PlayerBot playerBot;
        private Vector2Int mapRoot;

        private float halfTileSize = .5f;

        private Rigidbody2D theRigidbody;

        private bool isImmobilize = false;

        void Start()
        {
            theRigidbody = GetComponent<Rigidbody2D>();
            ServiceLocator.Resolve(out mapGenerator);
            mapWidth = mapGenerator.MapWidth;
            mapHeight = mapGenerator.MapHeight;
            mapRoot = Vector2Int.zero;
            aStar = new AStar(mapGenerator.MapWidth,mapGenerator.MapHeight,IsWalkable);
            playerBot = transform.GetComponent<PlayerBot>();
            var eventConsumer = EventSystems.EventConsumer.GetOrAttach(gameObject);
            eventConsumer.StartListening<DamageTakenData>(OnDamageTaken);
            eventConsumer.StartListening<BotGetCounteredData>(HandleGetCountered);
            eventConsumer.StartListening<BotCounterSuccessData>(OnCounterSuccessful);
        }

        private void OnDamageTaken(DamageTakenData data)
        {
            if (!data.damagedId.Equals(netId))
            {
                return;
            }
            
            Dash(data.atkDir * knockbackForce);
        }

        public void Immobilize(float time)
        {
            isImmobilize = true;
            stunnedIcon.SetActive(true);
            animator.PlayIdle();
            Invoke(nameof(RegainMobility), time);
        }

        public bool IsStuned() => isImmobilize;
        

        private void RegainMobility() 
        {
            stunnedIcon.SetActive(false);
            isImmobilize = false;
        }
        private void OnCounterSuccessful(BotCounterSuccessData data)
        {
            Dash(data.counterDir * counterSuccessDashDistance);
        }

        private void HandleGetCountered(BotGetCounteredData counterData)
        {
            Immobilize(counterData.immobilizeTime);
        }

        private void Dash(Vector2 vect)
        {
            theRigidbody.AddForce(vect * DASH_MULTIPLIER, ForceMode2D.Impulse);
        }

        ///END HARD CODE ZONE 

        private Vector2 IndexToWorldMiddleSquare(Vector2Int index)
        {
            return index + mapRoot + Vector2.one*halfTileSize;
        }
        private Vector2Int IndexToWorld(Vector2Int index)
        {
            return index + mapRoot;
        }

        private Vector2Int WorldToIndex(Vector2 world)
        {
            return new Vector2Int(Mathf.FloorToInt(world.x) - mapRoot.x, Mathf.FloorToInt(world.y) - mapRoot.y);
        }

        private bool IsWalkable(Vector2Int from, Vector2Int to)
        {
            {
                //if obstacle
                if (IsObstacle(to)) return false;
                
                //Check if move diagonal
                if ((from.x != to.x) && (from.y != to.y))
                {
                    if (IsObstacle(new Vector2Int(from.x,to.y)) || IsObstacle(new Vector2Int(to.x, from.y))) return false;
                }
            }
            return true;
        }
        
        private bool IsObstacle(Vector2Int index)
        {
            return mapGenerator.IsObstacle(index.x, index.y);
        }

        private void MoveBot()
        {
            if (isMoving && hasPath) 
            {
                if (!IsInRightPath()) {
                    ReplanPath(IndexToWorld(path[path.Count -1].index));
                    return;
                }
                if (Vector2.Distance(currentGoal,transform.position) < .05f)
                {
                    // transform.position = currentGoal;
                    Vector2Int playerPos = WorldToIndex(transform.position);
                    bool found = false;
                    for (int i=0; i < path.Count; i++)
                    {
                        if (path[i].index == playerPos)
                        {
                            if (i + 1 < path.Count)
                            {
                                currentNode = i;
                                // Debug.Log("pos is " +i);
                                currentGoal = IndexToWorldMiddleSquare(path[i+1].index);
                                found = true;
                                break;
                            }
                            else
                            {
                                // Debug.Log("arrive goal " + i );
                                isMoving = false;
                                return;
                            }
                        }
                    }
                    if (!found)
                    {
                        hasPath = false;
                        ReplanPath(IndexToWorld(path[path.Count -1].index));
                    }
                }

                Vector2 moveDir = currentGoal - (Vector2)transform.position;
                animator.SetMovementState(moveDir);
                theRigidbody.MovePosition(theRigidbody.position + 
                moveDir.normalized*speed*Time.fixedDeltaTime * (slowDownCount>0?(1- SlowDownPercentage):1));
            }
        }

        private bool IsInRightPath()
        {
            Vector2Int playerIndex= WorldToIndex(transform.position);
            if (currentNode >= path.Count || currentNode < 0) return false;
            if (playerIndex == path[currentNode].index || (currentNode + 1 < path.Count && playerIndex == path[currentNode + 1].index) ) return true;
            return false;
        }

        void FixedUpdate()
        {
            if (!hasAuthority) return;
            // if (path != null)
            // {
            //     foreach (PathFinding.Node node in path)
            //     {
            //         Debug.DrawLine(IndexToWorldMiddleSquare(node.index) -Vector2.one/10f,IndexToWorldMiddleSquare(node.index)+Vector2.one/10f, Color.green);
            //     }
            // }
            
            if (isImmobilize)
            {
                return;
            }

            MoveBot();
        }

        public void SetAnimator(BotAnimator anim) => animator = anim;
        public bool IsMoving => isMoving;
        public void startMoving() => isMoving = true;
        public void StopMoving()
        {
            isMoving = false;
            animator.PlayIdle();
        }
        
        void ReplanPath(Vector2 movePos)
        {
            if (SetMovePos(movePos))
            {
                startMoving();
            }
            playerBot.ResetFMS();
        }

        public bool SetMovePos(Vector2 movePos) 
        {         
            path = aStar.FindPath(WorldToIndex(transform.position),WorldToIndex(movePos));
            if (path != null)
            {
                length = path.Count;
                currentNode = 0;
                currentGoal = IndexToWorldMiddleSquare(path[currentNode].index);
                hasPath = true;
                return true;
            }
            hasPath = false;
            return false;
        }

        private int MAXTRY =20;
        private int tried;
        public void StartWandering()
        {
            Vector2Int ranDomIndex = Vector2Int.zero;
            Vector2Int currentPos = WorldToIndex(transform.position);
            tried = 0;
            while (tried < MAXTRY)
            {   
                ranDomIndex.x = Random.Range(0, mapWidth);
                ranDomIndex.y = Random.Range(0, mapHeight);

                path = aStar.FindPath(currentPos, ranDomIndex);
                if (path != null)
                {
                    length = path.Count;
                    currentNode = 0;
                    currentGoal = IndexToWorldMiddleSquare(path[currentNode].index);
                    hasPath = true;
                    startMoving();
                    return;
                }
                tried++;
            }
            hasPath = false;
        }

        public void SlowDown(float time)
        {
            StartCoroutine(SlowDownCoroutine(time));
            slowEffect.Play();
        }

        private IEnumerator SlowDownCoroutine(float time)
        {
            slowDownCount += 1;
            yield return new WaitForSeconds(time);
            slowDownCount -= 1;
            if (slowDownCount <= 0) slowEffect.Stop();
        }
    }
}
