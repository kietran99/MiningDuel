using System;
using System.Collections.Generic;

namespace EventSystems
{
    public class EventManager
    {
        #region SINGLETON
        public static EventManager Instance
        {
            get
            {
                if (instance != null) return instance;

                instance = new EventManager
                {
                    eventDictionary = new Dictionary<System.Type, IBaseEvent>()
                };

                return instance;
            }
        }
        private static EventManager instance;
        private EventManager() { }
        #endregion

        private Dictionary<Type, IBaseEvent> eventDictionary;

        public void StartListening<T>(Action listener) where T : IEventData
        {
            if (instance.eventDictionary.TryGetValue(typeof(T), out IBaseEvent publisher))
            {
                (publisher as VoidEvent).Add(listener);
                return;
            }
            
            publisher = new VoidEvent();
            instance.eventDictionary.Add(typeof(T), publisher);
            (publisher as VoidEvent).Add(listener);
        }

        public void StopListening<T>(Action listener) where T : IEventData
        {
            if (instance.eventDictionary.TryGetValue(typeof(T), out IBaseEvent publisher))
            {
                (publisher as VoidEvent).Remove(listener);
            }
        }

        public void TriggerEvent<T>() where T : IEventData
        {
            if (instance.eventDictionary.TryGetValue(typeof(T), out IBaseEvent publisher))
            {
                (publisher as VoidEvent).Invoke();
            }
        }
    
        public void StartListening<T>(Action<T> listener) where T : IEventData
        {
            if (instance.eventDictionary.TryGetValue(typeof(T), out IBaseEvent publisher))
            {
                (publisher as PackagedEvent<T>).Add(listener);
                return;
            }
            
            publisher = new PackagedEvent<T>();
            instance.eventDictionary.Add(typeof(T), publisher);
            (publisher as PackagedEvent<T>).Add(listener);
        }

        public void StopListening<T>(Action<T> listener) where T : IEventData
        {
            if (instance.eventDictionary.TryGetValue(typeof(T), out IBaseEvent publisher))
            {
                (publisher as PackagedEvent<T>).Remove(listener);
            }
        }

        public void TriggerEvent<T>(T eventData) where T : IEventData
        {
            if (instance.eventDictionary.TryGetValue(typeof(T), out IBaseEvent publisher))
            {
                (publisher as PackagedEvent<T>).Invoke(eventData);
            }
        }
    
        public void StartListening<EventT, T1>(Action<T1> listener) where EventT : IArgEventData
        {
            if (instance.eventDictionary.TryGetValue(typeof(EventT), out IBaseEvent publisher))
            {
                (publisher as ArgEvent<T1>).Add(listener);
                return;
            }
            
            publisher = new ArgEvent<T1>();
            instance.eventDictionary.Add(typeof(EventT), publisher);
            (publisher as ArgEvent<T1>).Add(listener);
        }

        public void StopListening<EventT, T1>(Action<T1> listener) where EventT : IArgEventData
        {
            if (instance.eventDictionary.TryGetValue(typeof(EventT), out IBaseEvent publisher))
            {
                (publisher as ArgEvent<T1>).Remove(listener);
            }
        }

        public void TriggerEvent<EventT, T1>(T1 eventData) where EventT : IArgEventData
        {
            if (instance.eventDictionary.TryGetValue(typeof(EventT), out IBaseEvent publisher))
            {
                (publisher as ArgEvent<T1>).Invoke(eventData);
            }
        }
    }
}

