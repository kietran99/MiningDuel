using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public class GoToClosestHome : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            var maybeHomePos = blackboard.Get<Vector3>("HomePos");
            return maybeHomePos
                .Match(
                    homePos =>
                    {                       
                        if (Vector3.Distance(homePos, actor.transform.position) <= 1f)
                        {
                            return BTNodeState.SUCCESS;
                        }

                        Vector2 moveDir = ((Vector2)homePos - new Vector2(actor.transform.position.x, actor.transform.position.y)).normalized * 5 * Time.deltaTime;
                        actor.transform.Translate(moveDir);
                        return BTNodeState.RUNNING;                        
                    },
                    () => BTNodeState.FAILURE
                );
        }
    }
}
