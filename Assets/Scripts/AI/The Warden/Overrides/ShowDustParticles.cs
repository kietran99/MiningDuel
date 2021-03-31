using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class ShowDustParticles : BTLeaf
    {       
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            return 
                blackboard
                    .Get<ParticleSystem>(WardenMacros.DUST_PARTICLES)
                    .Match(
                        dustParticles => 
                        {
                            dustParticles.Play();
                            return BTNodeState.SUCCESS;
                        },
                        () => BTNodeState.FAILURE
                    );
        }
    }
}
