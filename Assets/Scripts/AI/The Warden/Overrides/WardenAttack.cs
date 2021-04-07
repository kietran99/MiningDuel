using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class WardenAttack : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            return 
                blackboard
                    .Get<IWardenDamagable[]>(WardenMacros.DAMAGABLES)
                    .Map(
                        damagables => 
                        {
                            blackboard.Get<Animator>(WardenMacros.ANIMATOR).Match(animator => animator.SetTrigger(WardenMacros.SHOULD_ATTACK_TRIGGER), () => {});
                            blackboard.Get<ParticleSystem>(WardenMacros.ATTACK_PARTICLES).Match(particleSystem => particleSystem.Play(), () => {});
                            damagables.ForEach(damagable => damagable.TakeDamage());
                            return BTNodeState.SUCCESS;
                        }
                    );
        }
    }
}
