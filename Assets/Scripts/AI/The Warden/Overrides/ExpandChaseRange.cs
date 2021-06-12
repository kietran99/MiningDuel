using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class ExpandChaseRange : BTLeaf
    {  
        [SerializeField]
        private float expansionLength = 2f;

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            var deltaChaseRange = blackboard.NullableGet<float>(WardenMacros.DELTA_CHASE_RANGE);
            blackboard.Set(WardenMacros.DELTA_CHASE_RANGE, deltaChaseRange + expansionLength);
            return BTNodeState.SUCCESS;      
        }
    }
}