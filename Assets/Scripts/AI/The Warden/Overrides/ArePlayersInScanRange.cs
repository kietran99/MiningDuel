using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class ArePlayersInScanRange : BTLeaf
    {
        private readonly int MAX_PLAYERS = 4;

        [SerializeField]
        private float baseChaseRange = 7f;

        [SerializeField]
        private Transform[] players = null; // For testing targets

        private bool hasSetup = false;

        private Transform[] targets;

        private void Start() => targets = players;

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            if (!hasSetup)
            {
                blackboard.Set<float>(WardenMacros.CHASE_RANGE, baseChaseRange);
                hasSetup = true;
            }

            return blackboard
                .Get<float>(WardenMacros.CHASE_RANGE)
                .Match(
                    chaseRange => 
                    {
                        var chasablePlayers = targets.Filter(player => (actor.transform.position - player.position).sqrMagnitude <= chaseRange * chaseRange);
                        
                        BTLogger.Log("Number of Chasable Players: " + chasablePlayers.Length);

                        return chasablePlayers.Length > 0 ? BTNodeState.SUCCESS : BTNodeState.FAILURE;
                    },
                    () => BTNodeState.FAILURE
                );
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, baseChaseRange);
        }
    }
}
