using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public class IsAtHome : BTLeaf
    {
        [SerializeField]
        private Transform[] homes = null;

        protected override BTNodeState DecoratedTick(GameObject actor)
        {
            foreach (var home in homes)
            {
                if (Vector3.Distance(home.position, actor.transform.position) <= 1f)
                {
                    return BTNodeState.SUCCESS;
                }
            }

            return BTNodeState.FAILURE;
        }
    }
}
