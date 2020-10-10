using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventSystems
{
    public abstract class EventListener<T> : MonoBehaviour where T : IEventData
    {
        private Action<T> listener;

        protected virtual void Start()
        {
            
        }

        protected virtual void OnDestroy()
        {
            StopListening(listener);
        }

        protected void StartListening(Action<T> listener) 
        {
            this.listener = listener;
            EventManager.Instance.StartListening<T>(listener);
        }

        protected void StopListening(Action<T> listener)
        {
            EventManager.Instance.StopListening<T>(listener);
        }

        protected abstract void BindListener(Action<T> listener);
    }
}