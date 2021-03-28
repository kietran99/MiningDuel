using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public abstract class BTComposite : MonoBehaviour, IBTNode
    {
        protected IBTNode[] children;

        private void Start()
        {
            var childCount = transform.childCount;

            if (childCount >= 1)
            {
                children = new IBTNode[transform.childCount];
                for (int i = 0; i < childCount; i++) children[i] = transform.GetChild(i).GetComponent<IBTNode>();
                Debug.Log(children.Length);
                return;
            }
                      
            Debug.LogError("Behaviour Tree: Composite " + name + " must have at least 1 child.");
            gameObject.SetActive(false);
        }

        public abstract BTNodeState Tick(GameObject actor);
    }
}
