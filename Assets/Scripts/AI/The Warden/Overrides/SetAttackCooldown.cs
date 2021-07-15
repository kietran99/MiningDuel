using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class SetAttackCooldown : BTLeaf
    {
        [SerializeField]
        private float cooldown = 10f;

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            blackboard.Set<float>(WardenMacros.ATTACK_COOLDOWN, cooldown);
            return BTNodeState.SUCCESS;
        }
    }
}
