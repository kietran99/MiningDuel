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
        private WardenMove moveAssist;
        private Vector2 curGoal;
        private float distanceToNextDecision = 10f;
        private int attackAngle;

        public override void OnRootInit(BTBlackboard blackboard)
        {
            quadrants = blackboard.Get<Quadrant[]>(WardenMacros.QUADRANTS).Match(quadrants => quadrants, () => null);

            if (quadrants == null) gameObject.SetActive(false);  

            moveAssist = new WardenMove();
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
