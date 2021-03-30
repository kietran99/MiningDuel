using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class ChasePlayerWithHighestScore : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            
            return BTNodeState.FAILURE;
        }
    }
}