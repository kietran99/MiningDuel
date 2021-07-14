using UnityEngine;
using MD.AI.BehaviourTree;

namespace MD.AI.TheWarden
{
    public class WardenShouldChargeAttack : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            return blackboard.NullableGet<bool>(WardenMacros.SHOULD_CHARGE_ATK) ? BTNodeState.SUCCESS : BTNodeState.FAILURE;
        }
    }
}
