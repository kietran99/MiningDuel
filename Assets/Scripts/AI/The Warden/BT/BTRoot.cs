﻿using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public class BTRoot : MonoBehaviour
    {
        private readonly int ROOT_CHILD_COUNT = 1;

        [SerializeField]
        private GameObject actor = null;

        private IBTNode child;

        private void Start()
        {
            if (transform.childCount == ROOT_CHILD_COUNT)
            {
                child = transform.GetChild(0).GetComponent<IBTNode>();

                if (child == null)
                {
                    Debug.LogError("Behaviour Tree: " + name + " must have a child with IBTNode script attached");
                    gameObject.SetActive(false);
                }

                return;
            }
                
            Debug.LogError("Behaviour Tree: Root " + name + " can only has 1 child");
            gameObject.SetActive(false);
        }

        private void Update()
        {
            child.Tick(actor);
        }
    }
}
