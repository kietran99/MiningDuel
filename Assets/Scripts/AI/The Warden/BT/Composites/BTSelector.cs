﻿using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public class BTSelector : BTComposite
    {
        public override BTNodeState Tick(GameObject actor, BTBlackboard blackboard)
        {
            foreach (var child in children)
            {
                var res = child.Tick(actor, blackboard);

                if (!res.Equals(BTNodeState.FAILURE))
                {
                    BTLogger.Log(name + " - " + res);
                    return res;
                }
            }

            BTLogger.Log(name + " - FAILURE");
            return BTNodeState.FAILURE;
        }
    }
}