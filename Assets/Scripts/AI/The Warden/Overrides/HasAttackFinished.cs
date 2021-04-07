using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class HasAttackFinished : BTLeaf
    {
        private readonly string ATK_ANIM_NAME = "Base Layer.The_Warden_Attack";

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            return blackboard
                    .Get<Animator>(WardenMacros.ANIMATOR)
                    .Map(animator => animator.GetCurrentAnimatorStateInfo(0).IsName(ATK_ANIM_NAME) ? BTNodeState.SUCCESS : BTNodeState.FAILURE);
        }
    }
}
