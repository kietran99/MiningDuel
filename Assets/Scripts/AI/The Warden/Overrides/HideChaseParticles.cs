﻿using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class HideChaseParticles : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            return 
                blackboard
                    .Get<IWardenParticleController>(WardenMacros.PARTICLE_CONTROLLER)
                    .Map(particleController => 
                    {
                        particleController.HideChaseEffect();
                        return BTNodeState.SUCCESS;
                    });
        }
    }
}
