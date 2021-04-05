using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class CheckAttackCooldown : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            return 
                blackboard
                    .Get<float>(WardenMacros.ATTACK_COOLDOWN)
                    .Map(cooldown => cooldown <= 0f ? BTNodeState.FAILURE : BTNodeState.SUCCESS);
        }
    }
}
