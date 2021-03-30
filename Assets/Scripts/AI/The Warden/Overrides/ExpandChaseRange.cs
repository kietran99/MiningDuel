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
            return blackboard
                .Get<float>(WardenMacros.CHASE_RANGE)
                .Match(
                    chaseRange => 
                    {
                        blackboard.Set<float>(WardenMacros.CHASE_RANGE, chaseRange + expansionLength);
                        return BTNodeState.SUCCESS;
                    },
                    () => BTNodeState.FAILURE
                );         
        }
    }
}