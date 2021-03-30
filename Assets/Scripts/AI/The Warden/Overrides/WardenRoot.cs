using UnityEngine;
using MD.AI.BehaviourTree;

namespace MD.AI.TheWarden
{
    public class WardenRoot : BTRoot
    {
        [SerializeField]
        private float chaseRange = 3f;

        // [SerializeField]
        // private float chaseRangeWidenInterval = 10f;

        protected override void SetupAdditionalStates()
        {
            // blackboard.Set
            
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }
    }
}
