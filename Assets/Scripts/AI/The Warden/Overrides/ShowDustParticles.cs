using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class ShowDustParticles : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            actor.GetComponentInChildren<ParticleSystem>().Play();
            return BTNodeState.SUCCESS;
        }
    }
}
