using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class ShouldExpandChaseRange : BTLeaf
    {
        [SerializeField]
        private float chaseRangeExpandInterval = 10f;

        private float timeSinceLastExpansion = 0f;

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            timeSinceLastExpansion += Time.deltaTime;

            if (timeSinceLastExpansion < chaseRangeExpandInterval)
            {
                return BTNodeState.FAILURE;
            }

            timeSinceLastExpansion = 0f;
            return BTNodeState.SUCCESS;
        }
    }
}
