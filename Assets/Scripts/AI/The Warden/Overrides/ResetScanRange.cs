using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class ResetScanRange : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            blackboard.Set<float>(WardenMacros.DELTA_CHASE_RANGE, 0f);
            return BTNodeState.SUCCESS;
        }
    }
}
