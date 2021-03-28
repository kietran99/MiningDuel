using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public class IsCloseToHome : BTLeaf
    {
        [SerializeField]
        private Transform[] homes = null;

        [SerializeField]
        private float acceptableDistance = 5f;

        protected override BTNodeState DecoratedTick(GameObject actor)
        {
            foreach (var home in homes)
            {
                if (Vector3.Distance(home.position, actor.transform.position) <= acceptableDistance)
                {
                    return BTNodeState.SUCCESS;
                }
            }

            return BTNodeState.FAILURE;
        }
    }
}
