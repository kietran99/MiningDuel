using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class WardenAttackEffect : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            Play(blackboard);
            return BTNodeState.SUCCESS;
        }

        private void Play(BTBlackboard blackboard)
        {
            blackboard.Get<Animator>(WardenMacros.ANIMATOR).Match(animator => animator.SetTrigger(WardenMacros.SHOULD_ATTACK_TRIGGER), () => {});
            blackboard
                .Get<IWardenParticleController>(WardenMacros.PARTICLE_CONTROLLER)
                .Match(particleController => particleController.PlayAttackEffect(), () => {});
        }
    }
}
