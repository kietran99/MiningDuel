namespace MD.AI.BehaviourTree
{
    public interface IBTNode
    {
        void OnRootInit(BTBlackboard blackboard);
        BTNodeState Tick(UnityEngine.GameObject actor, BTBlackboard blackboard);
    }
}