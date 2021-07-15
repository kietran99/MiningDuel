using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class PlayWanderEffect : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            var moveDir = blackboard.NullableGet<Vector2>(WardenMacros.WANDER_DIR, true);
            blackboard.NullableGet<IWardenParticleController>(WardenMacros.PARTICLE_CONTROLLER).PlayWanderEffect(moveDir);
            return BTNodeState.SUCCESS;
        }
    }
}