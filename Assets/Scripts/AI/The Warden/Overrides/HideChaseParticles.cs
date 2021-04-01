using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class HideChaseParticles : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            return 
                blackboard
                    .Get<ParticleSystem>(WardenMacros.CHASE_PARTICLES)
                    .Match(
                        dustParticles => 
                        {
                            dustParticles.Stop();
                            return BTNodeState.SUCCESS;
                        },
                        () => BTNodeState.FAILURE
                    );
        }
    }
}
