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
            blackboard.Set<float>(WardenMacros.DELTA_CHASE_RANGE, expansionLength);
            return BTNodeState.SUCCESS;      
        }
    }
}