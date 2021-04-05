﻿using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class ShowChaseParticles : BTLeaf
    {       
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            return 
                blackboard
                    .Get<ParticleSystem>(WardenMacros.CHASE_PARTICLES)
                    .Map(dustParticles => 
                    {
                        dustParticles.Play();
                        return BTNodeState.SUCCESS;
                    });
        }
    }
}
