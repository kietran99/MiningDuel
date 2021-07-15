using UnityEngine;
using MD.AI.BehaviourTree;

namespace MD.AI.TheWarden
{
    public class WardenStartAttackCharge : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            blackboard.Set(WardenMacros.SHOULD_CHARGE_ATK, true);
            blackboard.NullableGet<IWardenAttackChargeIndicator>(WardenMacros.ATK_CHARGE_INDICATOR).Show();
            return BTNodeState.SUCCESS;
        }
    }
}
