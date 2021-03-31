using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class HideDustParticles : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            actor.GetComponentInChildren<ParticleSystem>().Stop();
            return BTNodeState.SUCCESS;
        }
    }
}
