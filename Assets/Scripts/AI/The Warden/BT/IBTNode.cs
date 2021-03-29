namespace MD.AI.BehaviourTree
{
    public interface IBTNode
    {
        BTNodeState Tick(UnityEngine.GameObject actor, BTBlackboard blackboard);
    }
}