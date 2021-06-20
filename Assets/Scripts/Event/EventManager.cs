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

        public async System.Threading.Tasks.Task TriggerEventAsync<T>(T eventData) where T : IEventData
        {
            if (instance.eventDictionary.TryGetValue(typeof(T), out IBaseEvent publisher))
            {
                await (publisher as PackagedEvent<T>).InvokeAsync(eventData);
            }
        }
    }
}

