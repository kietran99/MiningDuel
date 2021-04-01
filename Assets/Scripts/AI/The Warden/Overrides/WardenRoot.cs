using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class WardenRoot : BTRoot
    {
        [SerializeField]
        private Transform[] players = null; // FOR TESTING ONLY

        protected override void SetupAdditionalStates(BTBlackboard blackboard)
        {
            blackboard.Set<Transform[]>(WardenMacros.PLAYERS, players);
            blackboard.Set<ParticleSystem>(WardenMacros.CHASE_PARTICLES, actor.GetComponentInChildren<ParticleSystem>());         
        }
    }
}
