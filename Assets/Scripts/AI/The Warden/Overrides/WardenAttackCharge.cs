using UnityEngine;
using MD.AI.BehaviourTree;

namespace MD.AI.TheWarden
{
    public class WardenAttackCharge : BTLeaf
    {
        [SerializeField]
        private float chargeSeconds = .5f;

        private float elapsedCharge = 0f;
        private float scaleFactor = 0f;

        public override void OnRootInit(BTBlackboard blackboard)
        {
            scaleFactor = 1 / chargeSeconds;
            elapsedCharge = chargeSeconds;
        }

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            elapsedCharge -= Time.deltaTime;
            var scale = elapsedCharge * scaleFactor;
            blackboard.NullableGet<IWardenAttackChargeIndicator>(WardenMacros.ATK_CHARGE_INDICATOR).Scale(scale);
            var attackable = elapsedCharge <= 0f;
            var res = attackable ? BTNodeState.SUCCESS : BTNodeState.RUNNING;
            elapsedCharge = attackable ? chargeSeconds : elapsedCharge;
            return res;
        }
    }
}
