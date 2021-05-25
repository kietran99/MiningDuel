using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public class ChangeColor : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard)
        {
            actor.GetComponentInChildren<SpriteRenderer>().color = Color.red;
            return BTNodeState.SUCCESS;
        }
    }
}
