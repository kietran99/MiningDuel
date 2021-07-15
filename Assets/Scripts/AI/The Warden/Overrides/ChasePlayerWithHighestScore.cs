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
            return moveAssist.Move(actor.transform, blackboard.NullableGet<Transform>(WardenMacros.CHASE_TARGET).position, chaseSpeed);
        }
    }
}