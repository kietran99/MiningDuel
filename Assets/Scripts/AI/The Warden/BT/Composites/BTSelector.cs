using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public class BTSelector : BTComposite
    {
        public override BTNodeState Tick(GameObject actor, BTBlackboard blackboard)
        {
            foreach (var child in children)
            {
                var res = child.Tick(actor, blackboard);

                if (res != BTNodeState.FAILURE)
                {
                    BTLogger.LogResult(gameObject, res);
                    return res;
                }
            }

            BTLogger.LogResult(gameObject, BTNodeState.FAILURE);
            return BTNodeState.FAILURE;
        }
    }
}
