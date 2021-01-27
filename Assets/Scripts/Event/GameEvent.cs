using System;

namespace EventSystems
{
    public class VoidEvent : IBaseEvent
    {
        private event Action listeners;

        public void Add(Action listener) => listeners += listener;

        public void Remove(Action listener) => listeners -= listener;

        public void Invoke() => listeners?.Invoke();
    }

    public class PackagedEvent<T> : IBaseEvent where T : IEventData
    {
        private event Action<T> listeners;

        public void Add(Action<T> listener) => listeners += listener;

        public void Remove(Action<T> listener) => listeners -= listener;

        public void Invoke(T eventData) => listeners?.Invoke(eventData);        
    }  

    public class ArgEvent<T> : IBaseEvent
    {
        private event Action<T> listeners;

        public void Add(Action<T> listener) => listeners += listener;

        public void Remove(Action<T> listener) => listeners -= listener;

        public void Invoke(T eventData) => listeners?.Invoke(eventData);        
    }   
}