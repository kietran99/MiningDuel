using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class DecreaseAttackCooldown : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            var curCooldown = blackboard.NullableGet<float>(WardenMacros.ATTACK_COOLDOWN);
            blackboard.Set(WardenMacros.ATTACK_COOLDOWN, curCooldown - Time.deltaTime);
            return BTNodeState.SUCCESS;        
        }
    }
}
