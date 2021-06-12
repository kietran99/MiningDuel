using UnityEngine;
using MD.AI.BehaviourTree;

namespace MD.AI.TheWarden
{
    public class WardenStopAttackCharge : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            blackboard.Set(WardenMacros.SHOULD_CHARGE_ATK, false);
            blackboard.NullableGet<IWardenAttackChargeIndicator>(WardenMacros.ATK_CHARGE_INDICATOR).Hide();
            return BTNodeState.SUCCESS;
        }
    }
}
