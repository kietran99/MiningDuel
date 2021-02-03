using UnityEngine;
using Mirror;
using PathFinding;
using System.Collections.Generic;

namespace MD.AI
{
    public class BotMoveAction : NetworkBehaviour
    {
        public float speed = 3f;
        private RaycastHit2D[] rayArr = new RaycastHit2D[10];
        // private int resCount = 0;
        // private bool collideLeft = false;
        // private float collideLeftDistance = 0f;
        // private bool collideRigth = false;

        // private float collideRightDistance = 0f;
        // private bool collideAhead = false;
        private int currentNode;
        private Vector2 currentGoal;
        private List<PathFinding.Node> path;


        private bool isMoving = false;

        private BotAnimator animator;

        private AStar aStar;


        ///WARNING: HARD CODE A LOT HERE .TODO: GET DATA FROM MAP MANAGER
        private Vector2Int mapRoot = Vector2Int.one*-12;

        private float halfTileSize = .5f;
        void Start()
        {
            aStar = new AStar(24,20,IsWalkable);
        }

        ///END HARD CODE ZONE 

        private Vector2 IndexToWorld(Vector2Int index)
        {
            return index + mapRoot + Vector2.one*halfTileSize;
        }

        private Vector2Int WorldToIndex(Vector2 world)
        {
            return new Vector2Int(Mathf.FloorToInt(world.x) - mapRoot.x, Mathf.FloorToInt(world.y) - mapRoot.y);
        }

        private bool IsWalkable(Vector2Int from, Vector2Int to)
        {
            // if (IsIndexValid(to.x,to.y) && IsIndexValid(from.x,from.y))
            {
                //if obstacle
                if (IsObstacle(to)) return false;
                
                //Check if move diagonal
                if ((from.x != to.x) && (from.y != to.y))
                {
                    if (IsObstacle(new Vector2Int(from.x,to.y)) && IsObstacle(new Vector2Int(to.x, from.y))) return false;
                }
            }
            // else
            // {
            //     Debug.Log("index out of bound");
            //     return false;
            // }
            return true;
        }
        
        private bool IsObstacle(Vector2Int index)
        {
            return IsContainsObstacle(IndexToWorld(index));
        }

    ///GET OBSTACLE AT INDEX WAITING FOR MAP MANAGER API
    private RaycastHit2D[] hits; //Change this number to however many selectable objects you think you'll have layered on top of eachother. This is for performance reasons.
    private float rayStart = -1; //Start Raycast from this Z-Coordinate
    private float rayEnd = 1;  //End Raycast at this Z-Coordinate
    bool IsContainsObstacle(Vector2 Position)
    {
        hits = Physics2D.LinecastAll(new Vector3(Position.x,Position.y,rayStart),new Vector3(Position.x,Position.y,rayEnd));
        if(hits.Length > 0) //Only function if we actually hit something
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.CompareTag("obstacle")) return true;
            }
        }
        return false;
    }
    
    //////// 


        private void MoveBot()
        {
            if (isMoving) 
            {
                // resCount =  Physics2D.RaycastNonAlloc(transform.position, transform.forward, rayArr);
                
                
                if (Vector2.Distance(currentGoal,transform.position) < .1f)
                {
                    transform.position = currentGoal;
                    if (currentNode == path.Count -1)
                    {
                        isMoving = false;
                        return;
                    }
                    currentNode++;
                    currentGoal = IndexToWorld(path[currentNode].index);
                }

                Vector2 moveDir = currentGoal - (Vector2)transform.position;
                animator.SetMovementState(moveDir);
                transform.Translate(moveDir.normalized*speed*Time.fixedDeltaTime);
            }
        }

        void FixedUpdate()
        {
            if (!hasAuthority) return;
            MoveBot();
        }

        public void SetAnimator(BotAnimator anim) => animator = anim;
        public bool IsMoving => isMoving;
        public void startMoving() => isMoving = true;
        public bool SetMovePos(Vector2 movePos) 
        {
            path = aStar.FindPath(WorldToIndex(transform.position),WorldToIndex(movePos));
            if (path != null)
            {
                Debug.Log("found path");
                currentNode = 0;
                currentGoal = IndexToWorld(path[currentNode].index);
                return true;
            }
            return false;
        }
    }
}
