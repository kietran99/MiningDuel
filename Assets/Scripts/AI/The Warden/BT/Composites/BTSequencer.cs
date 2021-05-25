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

                if (!res.Equals(BTNodeState.SUCCESS))
                {
                    BTLogger.Log(name + " - " + res);
                    return res;
                }
            }

            BTLogger.Log(name + " - SUCCESS");
            return BTNodeState.SUCCESS;
        }
    }
}
