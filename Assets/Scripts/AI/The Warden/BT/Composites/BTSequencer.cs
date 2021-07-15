using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public class BTSequencer : BTComposite
    {
        public override BTNodeState Tick(GameObject actor, BTBlackboard blackboard)
        {
            foreach (var child in children)
            {
                var res = child.Tick(actor, blackboard);

                if (res != BTNodeState.SUCCESS)
                {
                    BTLogger.LogResult(gameObject, res);
                    return res;
                }
            }

            BTLogger.LogResult(gameObject, BTNodeState.SUCCESS);
            return BTNodeState.SUCCESS;
        }
    }
}
