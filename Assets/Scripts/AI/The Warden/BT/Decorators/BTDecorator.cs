using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public abstract class BTDecorator : MonoBehaviour, IBTNode
    {
        protected IBTNode child;

        public void OnRootInit(BTBlackboard blackboard) 
        {
            var childCount = transform.childCount;

            if (childCount != 1)
            {
                Debug.LogError("Behaviour Tree: Decorator " + name + " must have exactly 1 child.");
                gameObject.SetActive(false);
                return;
            }
                            
            child = transform.GetChild(0).GetComponent<IBTNode>();
            child.OnRootInit(blackboard);
        }

        public abstract BTNodeState Tick(GameObject actor, BTBlackboard blackboard);       
    }
}
