using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class ChasePlayerWithHighestScore : BTLeaf
    {
        [SerializeField]
        private float chaseSpeed = 5f;

        private WardenMove moveAssist = new WardenMove();

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            return 
                blackboard
                    .Get<Transform>(WardenMacros.CHASE_TARGET)
                    .Match(
                        target => moveAssist.Move(actor.transform, target.position, chaseSpeed),
                        () => BTNodeState.FAILURE
                    );
        }
    }
}