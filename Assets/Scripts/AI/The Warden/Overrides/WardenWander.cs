using UnityEngine;
using MD.AI.BehaviourTree;

namespace MD.AI.TheWarden
{
    public class WardenWander : BTLeaf
    {
        [SerializeField]
        private float distanceToNextDecision = 10f;

        [SerializeField]
        private float moveSpeed = 5f;

        private bool init;
        private WardenMove moveAssist;
        private Vector2 lastGoal;
        private int lastAngle = 0;
        private Quadrant[] quadrants;
        private Vector3 gLastActorPos;

        public override void OnRootInit(BTBlackboard blackboard)
        {
            quadrants = blackboard.NullableGet<Quadrant[]>(WardenMacros.QUADRANTS);

            if (quadrants == null) 
            {
                gameObject.SetActive(false);
            }

            moveAssist = new WardenMove();
            init = true;
        }

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            if (init)
            {
                (lastAngle, lastGoal) = FindNextGoal(actor.transform.position, 0, actor.transform.position);
                init = false;
            }

            var res = moveAssist.Move(actor.transform, lastGoal, moveSpeed);

            if (res == BTNodeState.SUCCESS)
            {
                (lastAngle, lastGoal) = FindNextGoal(actor.transform.position, lastAngle, lastGoal);
            }

            gLastActorPos = actor.transform.position;
            return res;
        }

        private (int, Vector2) FindNextGoal(Vector2 actorPos, int lastAngle, Vector2 lastGoal)
        {
            var movableQuadrants = quadrants.Filter(quadrant => quadrant.Movable(actorPos, distanceToNextDecision) && !quadrant.IsIn(lastAngle));
            var nextAngle = movableQuadrants.Length == 0 ? (lastAngle + 180) : movableQuadrants.Random().RandAngle();
            var angleInRad = nextAngle * Mathf.Deg2Rad;
            var nextGoal = new Vector2(distanceToNextDecision * Mathf.Cos(angleInRad) + lastGoal.x, distanceToNextDecision * Mathf.Sin(angleInRad) + lastGoal.y);
            return (nextAngle, nextGoal);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(gLastActorPos, distanceToNextDecision);
        }
    }
}
