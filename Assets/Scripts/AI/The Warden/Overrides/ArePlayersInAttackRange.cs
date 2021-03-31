using MD.AI.BehaviourTree;
using UnityEngine;

namespace MD.AI.TheWarden
{
    public class ArePlayersInAttackRange : BTLeaf
    {
        [SerializeField]
        private float baseAttackRange = 2f;

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            return BTNodeState.FAILURE;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, baseAttackRange);
        }
    }
}
