using UnityEngine;
using MD.AI.BehaviourTree;

namespace MD.AI.TheWarden
{
    public class WardenWander : BTLeaf
    {
        private class Quadrant
        {
            private int minAngle, maxAngle;
            private System.Func<Vector2, float, bool>[] limitCheckers;

            public Quadrant(int min, int max, System.Func<Vector2, float, bool>[] limitCheckers)
            {
                this.minAngle = min;
                this.maxAngle = max;
                this.limitCheckers = limitCheckers;
            }

            public bool IsIn(int angle) => minAngle <= angle && angle < maxAngle;

            public bool Movable(Vector2 pos, float moveDist) => !limitCheckers.Find(checker => checker(pos, moveDist)).HasValue;

            public int Random() => UnityEngine.Random.Range(minAngle, maxAngle);
        }

        public UnityEngine.Tilemaps.Tilemap map; // Used as a temporary to assign values to topRightLimit & botLeftLimit

        private readonly float ACCEPTABLE_GOAL_DIST = 0.2f;
        
        [SerializeField]
        private float distanceToNextDecision = 10f;

        private Vector2 topRightLimit, botLeftLimit;
        private float moveSpeed = 5f;

        private Vector2 curGoal = new Vector2(0f, 0f);
        private int lastAngle = 0;
        private Quadrant[] quadrants;

        private void Start()
        {
            topRightLimit = map.localBounds.max - new Vector3(.5f, .5f, 0f);
            botLeftLimit = map.localBounds.min + new Vector3(.5f, .5f, 0f);

            System.Func<Vector2, float, bool> IsNearTopLimit = (Vector2 pos, float moveDist) => pos.y + moveDist >= topRightLimit.y;
            System.Func<Vector2, float, bool> IsNearBotLimit = (Vector2 pos, float moveDist) => pos.y - moveDist <= botLeftLimit.y;
            System.Func<Vector2, float, bool> IsNearRightLimit = (Vector2 pos, float moveDist) => pos.x + moveDist >= topRightLimit.x;
            System.Func<Vector2, float, bool> IsNearLeftLimit = (Vector2 pos, float moveDist) => pos.x - moveDist <= botLeftLimit.x; 

            quadrants = new Quadrant[4]
            {
                new Quadrant(0, 90, new System.Func<Vector2, float, bool>[2] { IsNearTopLimit, IsNearRightLimit }),
                new Quadrant(90, 180, new System.Func<Vector2, float, bool>[2] { IsNearTopLimit, IsNearLeftLimit }),
                new Quadrant(180, 270, new System.Func<Vector2, float, bool>[2] { IsNearBotLimit, IsNearLeftLimit }),
                new Quadrant(270, 360, new System.Func<Vector2, float, bool>[2] { IsNearBotLimit, IsNearRightLimit })
            };
        }

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            var actorPos = actor.transform.position;
            if (IsAtGoal(actorPos, curGoal))
            {
                curGoal = PickNewGoal(actorPos, distanceToNextDecision, curGoal);
                return BTNodeState.SUCCESS;
            }

            Vector2 moveDir = (curGoal - new Vector2(actorPos.x, actorPos.y)).normalized * moveSpeed * Time.deltaTime;
            actor.transform.Translate(moveDir);        

            return BTNodeState.RUNNING;
        }

        private bool IsAtGoal(Vector2 curPos, Vector2 goal) => Mathf.Pow(curPos.x - goal.x, 2) + Mathf.Pow(curPos.y - goal.y, 2) <= ACCEPTABLE_GOAL_DIST;

        private Vector2 PickNewGoal(Vector2 actorPos, float moveDist, Vector2 curGoal)
        {
            var randAngle = RandomAngle(actorPos, moveDist, lastAngle);
            lastAngle = randAngle;
            var radianRandAngle = randAngle * Mathf.Deg2Rad;
            return new Vector2(distanceToNextDecision * Mathf.Cos(radianRandAngle) + curGoal.x, distanceToNextDecision * Mathf.Sin(radianRandAngle) + curGoal.y);
        }

        private int RandomAngle(Vector2 actorPos, float moveDist, int lastAngle)
        {
            var movableQuadrants = quadrants.Filter(quadrant => quadrant.Movable(actorPos, moveDist) && !quadrant.IsIn(lastAngle));
            return movableQuadrants.Random().Random();   
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, distanceToNextDecision);
        }
    }
}
