using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public class GoToMousePosition : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            var maybeMousePos = blackboard.Get<Vector2>("MousePos"); 

            return maybeMousePos
                .Match(
                    mousePos => 
                    {
                        Vector2 moveDir = (mousePos - new Vector2(actor.transform.position.x, actor.transform.position.y)).normalized * 5 * Time.deltaTime;
                        actor.transform.Translate(moveDir);

                        return Vector3.Distance(actor.transform.position, mousePos) <= 1f ? BTNodeState.SUCCESS : BTNodeState.RUNNING;
                    },
                    () => BTNodeState.FAILURE
                );
        }
    }
}
