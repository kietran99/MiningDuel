﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventSystems
{
    [CreateAssetMenu(fileName="Event Channel", menuName="Events/Event Channel")]
    public class EventChannel : ScriptableObject
    {
        private Dictionary<Type, IBaseEvent> eventDictionary = new Dictionary<Type, IBaseEvent>();

        public void StartListening<T>(Action<T> listener) where T : IEventData
        {
            if (eventDictionary.TryGetValue(typeof(T), out IBaseEvent publisher))
            {
                (publisher as GameEvent<T>).Add(listener);
                return;
            }
            
            publisher = new GameEvent<T>();
            eventDictionary.Add(typeof(T), publisher);
            (publisher as GameEvent<T>).Add(listener);
        }

        public void StopListening<T>(Action<T> listener) where T : IEventData
        {
            if (eventDictionary.TryGetValue(typeof(T), out IBaseEvent publisher))
            {
                (publisher as GameEvent<T>).Remove(listener);
            }
        }

        public void TriggerEvent<T>(T eventData) where T : IEventData
        {
            if (eventDictionary.TryGetValue(typeof(T), out IBaseEvent publisher))
            {
                (publisher as GameEvent<T>).Invoke(eventData);
            }
        }
    }
}
