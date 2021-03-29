using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public class IsCloseToHome : BTLeaf
    {
        [SerializeField]
        private Transform[] homes = null;

        [SerializeField]
        private float acceptableDistance = 5f;

        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            foreach (var home in homes)
            {
                if (Vector3.Distance(home.position, actor.transform.position) <= acceptableDistance)
                {
                    blackboard.Set<Vector3>("HomePos", home.position);
                    return BTNodeState.SUCCESS;
                }
            }

            return BTNodeState.FAILURE;
        }
    }
}
