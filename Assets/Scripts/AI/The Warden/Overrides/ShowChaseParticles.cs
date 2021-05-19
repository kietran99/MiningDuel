using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class ShowChaseParticles : BTLeaf
    {       
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            return 
                blackboard
                    .Get<IWardenParticleController>(WardenMacros.PARTICLE_CONTROLLER)
                    .Map(particleController => 
                    {
                        var targetDir = blackboard.Get<Transform>(WardenMacros.CHASE_TARGET).Match(target => -actor.transform.position + target.position, () => Vector3.zero);
                        particleController.PlayChaseEffect(targetDir);
                        return BTNodeState.SUCCESS;
                    });
        }
    }
}
