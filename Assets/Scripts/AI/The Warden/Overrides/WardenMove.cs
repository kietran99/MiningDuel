using UnityEngine;
using MD.AI.BehaviourTree;

namespace MD.AI.TheWarden
{
    public class WardenMove
    {
        private readonly float ACCEPTABLE_GOAL_DIST = 0.2f;

        public BTNodeState Move(Transform actor, Vector2 goal, float moveSpeed)
        {
            var actorPos = actor.transform.position;
            if (IsAtGoal(actorPos, goal))
            {
                return BTNodeState.SUCCESS;
            }

            Vector2 moveDir = (goal - new Vector2(actorPos.x, actorPos.y)).normalized * moveSpeed * Time.deltaTime;
            actor.transform.Translate(moveDir);        

            return BTNodeState.RUNNING;
        }

        private bool IsAtGoal(Vector2 curPos, Vector2 goal) => Mathf.Pow(curPos.x - goal.x, 2) + Mathf.Pow(curPos.y - goal.y, 2) <= ACCEPTABLE_GOAL_DIST;
    }
}
