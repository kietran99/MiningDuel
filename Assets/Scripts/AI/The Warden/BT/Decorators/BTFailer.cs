using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public class BTFailer : BTDecorator
    {
        public override BTNodeState Tick(GameObject actor, BTBlackboard blackboard)
        {
            var res = child.Tick(actor, blackboard);
            return res == BTNodeState.RUNNING ? BTNodeState.RUNNING : BTNodeState.FAILURE;
        }
    }
}
