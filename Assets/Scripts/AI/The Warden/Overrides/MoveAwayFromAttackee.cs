using MD.AI.BehaviourTree;
using UnityEngine;

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
        private System.Collections.Generic.List<Quadrant> movableQuadrants;

        public override void OnRootInit(BTBlackboard blackboard)
        {
            quadrants = blackboard.NullableGet<Quadrant[]>(WardenMacros.QUADRANTS);

            if (quadrants == null) 
            {
                gameObject.SetActive(false); 
            } 

            moveAssist = new WardenMove();
            movableQuadrants = new System.Collections.Generic.List<Quadrant>(quadrants.Length);

            var angleInRad = quadrants.Random().RandAngle() * Mathf.Deg2Rad;
            curGoal = new Vector2(distanceToNextDecision * Mathf.Cos(angleInRad) + curGoal.x, distanceToNextDecision * Mathf.Sin(angleInRad) + curGoal.y);
        }

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            var res = moveAssist.Move(actor.transform, curGoal, moveAwaySpeed);

            if (res == BTNodeState.SUCCESS)
            {
                var atkPos = blackboard.NullableGet<Vector3>(WardenMacros.ATK_POS);
                attackAngle = Mathf.FloorToInt(Vector2.SignedAngle(Vector2.right, atkPos - actor.transform.position));
                
                movableQuadrants.Clear();
                for (int i = 0; i < quadrants.Length; i++)
                {
                    if (quadrants[i].Movable(actor.transform.position, distanceToNextDecision) && !quadrants[i].IsIn(attackAngle))
                    {
                        movableQuadrants.Add(quadrants[i]);
                    }
                }

                var angleInRad = (movableQuadrants.Count != 0 ? movableQuadrants.Random().RandAngle() : attackAngle) * Mathf.Deg2Rad;
                curGoal = new Vector2(distanceToNextDecision * Mathf.Cos(angleInRad) + curGoal.x, distanceToNextDecision * Mathf.Sin(angleInRad) + curGoal.y);
            }

            return BTNodeState.SUCCESS;
        }
    }
}
