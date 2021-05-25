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
            blackboard
                .Get<float>(WardenMacros.DELTA_CHASE_RANGE)
                .Match(
                    deltaChaseRange => blackboard.Set<float>(WardenMacros.DELTA_CHASE_RANGE, deltaChaseRange + expansionLength),
                    () => {}
                );
            return BTNodeState.SUCCESS;      
        }
    }
}