using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventSystems
{
    public abstract class EventListener : MonoBehaviour 
    {       
        private Dictionary<Type, Action<IEventData>> events;

        // protected virtual void Start()
        // {
            
        // }

        // protected virtual void OnDestroy()
        // {
        //     StopListening(listener);
        // }

        // protected void StartListening(Type type, Action<IEventData> listener)
        // {
        //     events.Add(type, listener);
        //     EventManager.Instance.StartListening((Action<type.MakeGenericType()>)listener);
        // }

        // protected void StopListening(Action<T> listener)
        // {
        //     EventManager.Instance.StopListening<T>(listener);
        // }

        // protected abstract void BindListener(Action<T> listener);
    }
}