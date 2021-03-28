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

            Debug.LogError("Behaviour Tree: Leaf " + name + " can not have any child.");
            gameObject.SetActive(false);
        }

        public BTNodeState Tick(GameObject actor)
        {
            var res = DecoratedTick(actor);
            Debug.Log(name + " - " + res);
            return res;
        }

        protected abstract BTNodeState DecoratedTick(GameObject actor);
    }
}
