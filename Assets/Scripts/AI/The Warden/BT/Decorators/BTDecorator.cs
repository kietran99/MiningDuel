﻿using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public abstract class BTDecorator : MonoBehaviour, IBTNode
    {
        protected IBTNode child;

        private void Start()
        {
            var childCount = transform.childCount;

            if (childCount == 1)
            {
                child = transform.GetChild(0).GetComponent<IBTNode>();
                return;
            }
                      
            Debug.LogError("Behaviour Tree: Decorator " + name + " must have exactly 1 child.");
            gameObject.SetActive(false);
        }

        public void OnRootInit(BTBlackboard blackboard) 
        {
            child.OnRootInit(blackboard);
        }

        public abstract BTNodeState Tick(GameObject actor, BTBlackboard blackboard);       
    }
}