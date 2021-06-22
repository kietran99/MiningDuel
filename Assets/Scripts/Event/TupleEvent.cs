using System;

namespace EventSystems
{
    public class TupleEvent<T> : IBaseEvent where T : struct
    {
        private Action<T> listeners;

        public void Publish(T data) => listeners.Invoke(data);

        public void Subscribe(System.Action<T> callback) { listeners += callback; }

        public void Subscribe(System.Action callback) { listeners += _ => callback(); }
    }
}
