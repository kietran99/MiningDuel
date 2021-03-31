using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public abstract class BTComposite : MonoBehaviour, IBTNode
    {
        protected IBTNode[] children;

        private void Start()
        {
            var childCount = transform.childCount;

            if (childCount < 1)
            {
                Debug.LogError("Behaviour Tree: Composite " + name + " must have at least 1 child.");
                gameObject.SetActive(false);
                return;
            }
                      
            var children = new System.Collections.Generic.List<IBTNode>();

            for (int i = 0; i < childCount; i++) 
            {
                var childTransform = transform.GetChild(i);

                if (!childTransform.gameObject.activeInHierarchy)
                {          
                    continue;
                }

                children.Add(childTransform.GetComponent<IBTNode>());
            }   

            this.children = children.ToArray();    
        }

        public void OnRootInit(BTBlackboard blackboard)
        {
            children.ForEach(child => child.OnRootInit(blackboard));
        }

        public abstract BTNodeState Tick(GameObject actor, BTBlackboard blackboard);
    }
}
