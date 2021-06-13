using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class WardenAttack : BTLeaf
    {
        [SerializeField]
        private int power = 40;

        [SerializeField]
        private float baseAttackRange = 1.5f;

        private float sqrBaseAtkRange = 0f;

        public override void OnRootInit(BTBlackboard blackboard)
        {
            sqrBaseAtkRange = baseAttackRange * baseAttackRange;
        }

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            blackboard.Set(WardenMacros.ATK_POS, actor.transform.position);

            blackboard
                .NullableGet<Transform[]>(WardenMacros.PLAYERS)
                .ForEach(
                    player => 
                    {
                        if ((actor.transform.position - player.transform.position).sqrMagnitude > sqrBaseAtkRange)
                        {
                            return;
                        }

                        player.GetComponent<IWardenDamagable>()?.TakeWardenDamage(power);
                    });

            return BTNodeState.SUCCESS;
        }
    }
}
