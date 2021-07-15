using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public abstract class BTLeaf : MonoBehaviour, IBTNode
    {
        private void Start()
        {
            if (transform.childCount == 0)
            {
                return;
            }

            Debug.LogError("Behaviour Tree: Leaf " + name + " can not have any child");
            gameObject.SetActive(false);
        }

        public BTNodeState Tick(GameObject actor, BTBlackboard blackboard)
        {
            var res = DecoratedTick(actor, blackboard);
            BTLogger.LogResult(gameObject, res);
            return res;
        }

        public virtual void OnRootInit(BTBlackboard blackboard) {}

        protected abstract BTNodeState DecoratedTick(GameObject actor, BTBlackboard blackboard);
    }
}
