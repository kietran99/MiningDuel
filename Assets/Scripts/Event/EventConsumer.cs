using System.Collections.Generic;
using UnityEngine;

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
            private System.Action<T> listener;

            public EventConnection(System.Action<T> listener)
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

        void OnDestroy()
        {
            connections.ForEach(conn => conn.Disconnect());
        }

        public void StartListening<T>(System.Action<T> listener) where T : IEventData
        {
            connections.Add(new EventConnection<T>(listener));
        }
    }
}
