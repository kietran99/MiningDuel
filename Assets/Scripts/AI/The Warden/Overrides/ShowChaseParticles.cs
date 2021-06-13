using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class ShowChaseParticles : BTLeaf
    {       
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            var targetDir = blackboard.NullableGet<Transform>(WardenMacros.CHASE_TARGET).position - actor.transform.position;
            blackboard.NullableGet<IWardenParticleController>(WardenMacros.PARTICLE_CONTROLLER).PlayChaseEffect(targetDir);
            return BTNodeState.SUCCESS;
        }
    }
}
