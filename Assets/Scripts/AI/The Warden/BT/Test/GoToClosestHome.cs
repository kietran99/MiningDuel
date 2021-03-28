using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public class GoToClosestHome : BTLeaf
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
                    if (Vector3.Distance(home.position, actor.transform.position) <= 1f)
                    {
                        return BTNodeState.SUCCESS;
                    }

                    Vector2 homePos = home.position;
                    Vector2 moveDir = (homePos - new Vector2(actor.transform.position.x, actor.transform.position.y)).normalized * 5 * Time.deltaTime;
                    actor.transform.Translate(moveDir);
                    return BTNodeState.RUNNING;
                }
            }

            return BTNodeState.FAILURE;
        }
    }
}
