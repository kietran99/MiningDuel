using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Jobs;

namespace EventSystems
{
    public class PackagedEvent<T> : IBaseEvent where T : IEventData
    {
        private event Action<T> listeners;

        public void Add(Action<T> listener) => listeners += listener;

        public void Remove(Action<T> listener) => listeners -= listener;

        public void Invoke(T eventData) => listeners?.Invoke(eventData);

        public async Task InvokeAsync(T eventData)
        {
            await Task.Run(() => listeners?.Invoke(eventData));
        }
    }
}