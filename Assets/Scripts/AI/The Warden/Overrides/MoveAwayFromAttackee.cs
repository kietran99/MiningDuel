using MD.AI.BehaviourTree;
using UnityEngine;
using System;

namespace MD.AI.TheWarden
{
    public class MoveAwayFromAttackee : BTLeaf
    {
        [SerializeField]
        private float moveAwaySpeed = 15f;

        private Quadrant[] quadrants;
        private WardenMove moveAssist = new WardenMove();
        private Vector2 curGoal;
        private int lastAngle = 0;
        private float distanceToNextDecision = 10f;
        private int attackAngle;

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
        }

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            var res = curGoal != null ? moveAssist.Move(actor.transform, curGoal, moveAwaySpeed) : BTNodeState.SUCCESS;

            if (res.Equals(BTNodeState.SUCCESS))
            {
                var attackeePos = blackboard.Get<Transform>(WardenMacros.CHASE_TARGET).Match(attackee => attackee.position, () => Vector3.zero);
                attackAngle = Mathf.FloorToInt(Vector2.SignedAngle(Vector2.right, attackeePos - actor.transform.position));
                var movableQuadrants = quadrants.Filter(quadrant => quadrant.Movable(actor.transform.position, distanceToNextDecision) && !quadrant.IsIn(attackAngle));
                var angleInRad = (movableQuadrants.Length != 0 ? movableQuadrants.Random().RandAngle() : attackAngle) * Mathf.Deg2Rad;
                curGoal = new Vector2(distanceToNextDecision * Mathf.Cos(angleInRad) + curGoal.x, distanceToNextDecision * Mathf.Sin(angleInRad) + curGoal.y);
            }

            return BTNodeState.SUCCESS;
        }

    }
}
