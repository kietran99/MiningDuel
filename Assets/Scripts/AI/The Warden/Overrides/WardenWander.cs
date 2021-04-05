using UnityEngine;
using MD.AI.BehaviourTree;
using System;

namespace MD.AI.TheWarden
{
    public class Quadrant
    {
        private int minAngle, maxAngle;
        private Func<Vector2, float, bool>[] limitCheckers;

        public Quadrant(int min, int max, Func<Vector2, float, bool>[] limitCheckers)
        {
            this.minAngle = min;
            this.maxAngle = max;
            this.limitCheckers = limitCheckers;
        }

        public bool IsIn(int angle) => minAngle <= angle && angle < maxAngle;

        public bool Movable(Vector2 pos, float moveDist) => !limitCheckers.Find(checker => checker(pos, moveDist)).HasValue;

        public int RandAngle() => UnityEngine.Random.Range(minAngle, maxAngle);
    }

    public class WardenWander : BTLeaf
    {
        [SerializeField]
        private float distanceToNextDecision = 10f;

        [SerializeField]
        private float moveSpeed = 5f;

        private WardenMove moveAssist;
        private Vector2 curGoal = new Vector2(0f, 0f);
        private int lastAngle = 0;
        private Quadrant[] quadrants;

        public override void OnRootInit(BTBlackboard blackboard)
        {
            var (topRightLimit, botLeftLimit) = blackboard
                                                    .Get<(Vector2, Vector2)>(WardenMacros.MAP_LIMITS)
                                                    .Match(limits => limits, () => (Vector2.zero, Vector2.zero));

            Func<Vector2, float, bool> IsNearTopLimit = (pos, moveDist) => pos.y + moveDist >= topRightLimit.y;
            Func<Vector2, float, bool> IsNearBotLimit = (pos, moveDist) => pos.y - moveDist <= botLeftLimit.y;
            Func<Vector2, float, bool> IsNearRightLimit = (pos, moveDist) => pos.x + moveDist >= topRightLimit.x;
            Func<Vector2, float, bool> IsNearLeftLimit = (pos, moveDist) => pos.x - moveDist <= botLeftLimit.x; 

            quadrants = new Quadrant[4]
            {
                new Quadrant(0, 90, new Func<Vector2, float, bool>[2] { IsNearTopLimit, IsNearRightLimit }),
                new Quadrant(90, 180, new Func<Vector2, float, bool>[2] { IsNearTopLimit, IsNearLeftLimit }),
                new Quadrant(180, 270, new Func<Vector2, float, bool>[2] { IsNearBotLimit, IsNearLeftLimit }),
                new Quadrant(270, 360, new Func<Vector2, float, bool>[2] { IsNearBotLimit, IsNearRightLimit })
            };

            moveAssist = new WardenMove();
        }

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            var res = moveAssist.Move(actor.transform, curGoal, moveSpeed);

            if (res.Equals(BTNodeState.SUCCESS))
            {
                var movableQuadrants = quadrants.Filter(quadrant => quadrant.Movable(actor.transform.position, distanceToNextDecision) && !quadrant.IsIn(lastAngle));
                lastAngle = movableQuadrants.Random().RandAngle();
                var angleInRad = lastAngle * Mathf.Deg2Rad;
                curGoal = new Vector2(distanceToNextDecision * Mathf.Cos(angleInRad) + curGoal.x, distanceToNextDecision * Mathf.Sin(angleInRad) + curGoal.y);
            }

            return res;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, distanceToNextDecision);
        }
    }
}
