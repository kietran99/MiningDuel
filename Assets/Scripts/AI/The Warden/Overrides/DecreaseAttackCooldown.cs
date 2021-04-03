using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class DecreaseAttackCooldown : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            return blackboard
                .Get<float>(WardenMacros.ATTACK_COOLDOWN)
                .Match(
                    cooldown => 
                    {
                        blackboard.Set<float>(WardenMacros.ATTACK_COOLDOWN, cooldown - Time.deltaTime);
                        return BTNodeState.SUCCESS;
                    },
                    () => BTNodeState.FAILURE
                );          
        }
    }
}
