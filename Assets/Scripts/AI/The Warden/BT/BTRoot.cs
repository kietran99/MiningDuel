using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public class BTRoot : MonoBehaviour
    {
        private readonly int ROOT_CHILD_COUNT = 1;

        [SerializeField]
        private bool shouldLog = true;

        [SerializeField]
        private GameObject actor = null;

        [SerializeField]
        private int blackboardCapacity = 1;

        protected BTBlackboard blackboard;
        private IBTNode child;

        private void Start()
        {
            BTLogger.active = shouldLog;

            if (transform.childCount != ROOT_CHILD_COUNT)
            {
                Debug.LogError("Behaviour Tree: Root " + name + " can only has 1 child");
                gameObject.SetActive(false);
                return;
            }
                           
            var childTransform = transform.GetChild(0);

            if (!childTransform.gameObject.activeInHierarchy)
            {
                Debug.LogError("Behaviour Tree: " + childTransform.name + " must be active");
                gameObject.SetActive(false);
            }

            child = childTransform.GetComponent<IBTNode>();

            if (child == null)
            {
                Debug.LogError("Behaviour Tree: " + name + " must have a child with IBTNode script attached");
                gameObject.SetActive(false);
            }

            blackboard = new BTBlackboard(blackboardCapacity);
            SetupAdditionalStates(blackboard);
            child.OnRootInit(blackboard);
        }

        private void Update()
        {
            child.Tick(actor, blackboard);

            #if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.LeftShift)) blackboard.Log();
            #endif
        }

        protected virtual void SetupAdditionalStates(BTBlackboard blackboard) {}
    }
}
