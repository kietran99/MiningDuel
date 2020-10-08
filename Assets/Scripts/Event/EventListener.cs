using MD.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventSystems
{
    public abstract class EventListener : MonoBehaviour
    {
        

        protected virtual void Start()
        {
            
        }

        protected virtual void OnDestroy()
        {
            
        }

        protected void StartListening<T>(Action<T> listener) where T : IEventData 
        {
            EventManager.Instance.StartListening<T>(listener);
        }

        protected void StopListening<T>(Action<T> listener) where T : IEventData
        {
            EventManager.Instance.StopListening<T>(listener);
        }
    }
}