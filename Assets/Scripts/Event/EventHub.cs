using System;
using System.Collections.Generic;

namespace EventSystems
{
    public class EventHub
    {
        #region SINGLETON
        public static EventHub Instance
        {
            get
            {
                if (instance != null) 
                {
                    return instance;
                }

                instance = new EventHub
                {
                    eventDict = new Dictionary<System.Type, IBaseEvent>()
                };

                return instance;
            }
        }
        private static EventHub instance;
        private EventHub() { }
        #endregion

        private Dictionary<Type, IBaseEvent> eventDict;

        public T Get<T>() where T : IBaseEvent, new()
        {
            if (instance.eventDict.TryGetValue(typeof(T), out IBaseEvent theEvent))
            {
                return (T) theEvent;
            }
            
            theEvent = new T();
            instance.eventDict.Add(typeof(T), theEvent);
            return (T) theEvent;
        }
    }
}
