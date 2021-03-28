using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public class ChangeColor : BTLeaf
    {
        protected override BTNodeState DecoratedTick(GameObject actor)
        {
            actor.GetComponentInChildren<SpriteRenderer>().color = Color.red;
            return BTNodeState.SUCCESS;
        }
    }
}
