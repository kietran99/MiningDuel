using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class HideWanderEffect : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            blackboard.NullableGet<IWardenParticleController>(WardenMacros.PARTICLE_CONTROLLER).HideWanderEffect();
            return BTNodeState.SUCCESS;
        }
    }
}