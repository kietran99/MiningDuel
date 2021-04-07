using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class HasAttackFinished : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            return blackboard
                    .Get<Animator>(WardenMacros.ANIMATOR)
                    .Map(animator => 
                    {
                        var finishedAnim = animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.The_Warden_Attack");
                        return finishedAnim ? BTNodeState.SUCCESS : BTNodeState.FAILURE;
                    });
        }
    }
}
