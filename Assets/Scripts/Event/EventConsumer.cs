using System.Collections.Generic;
using UnityEngine;
using System;

namespace EventSystems
{
    [DisallowMultipleComponent]
    public class EventConsumer : MonoBehaviour
    {
        private interface IEventConnection
        {
            void Disconnect();
        }

        private class EventConnection<T> : IEventConnection where T : IEventData
        {
            private Action<T> listener;

            public EventConnection(Action<T> listener)
            {
                this.listener = listener;
                EventManager.Instance.StartListening<T>(listener);
            }

            public void Disconnect()
            {
                EventManager.Instance.StopListening<T>(listener);
            }
        }

        private List<IEventConnection> connections = new List<IEventConnection>();

        public static EventConsumer Attach(GameObject listenerGO) => listenerGO.AddComponent<EventConsumer>();

        public static EventConsumer GetOrAttach(GameObject listenerGO)
        {
            var maybeEventConsumer = listenerGO.GetComponent<EventConsumer>();
            return maybeEventConsumer == null ? listenerGO.AddComponent<EventConsumer>() : maybeEventConsumer;
        }

        private void OnDestroy()
        {
            connections.ForEach(conn => conn.Disconnect());
        }

        public void StartListening<T>(Action<T> listener) where T : IEventData
        {
            connections.Add(new EventConnection<T>(listener));
        }

        public void StartListening<T>(Action listener) where T : IEventData
        {
            StartListening<T>(eventData => listener());
        }

        public void StartListening<T, TMap>(Action<TMap> listener, Func<T, TMap> mapFn) where T : IEventData
        {
            StartListening<T>(eventData => listener(mapFn(eventData)));
        }

        public void StartListening<T>(Action<T> listener, Predicate<T> pred) where T : IEventData
        {
            StartListening<T>(eventData => { if (pred(eventData)) listener(eventData); });
        }

        public void StartListening<T, TMap>(Action<TMap> listener, Func<T, TMap> mapFn, Predicate<T> pred) where T : IEventData
        {
            StartListening<T>(eventData => { if (pred(eventData)) listener(mapFn(eventData)); });
        }
    }
}
