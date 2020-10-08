using System;

namespace EventSystems
{
    public class GameEvent<T> : IBaseEvent where T : IEventData
    {
        private event Action<T> listeners;

        public void Add(Action<T> listener) => listeners += listener;

        public void Remove(Action<T> listener) => listeners -= listener;

        public void Invoke(T eventData) => listeners?.Invoke(eventData);
    }
}