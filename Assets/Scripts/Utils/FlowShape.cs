using System;

namespace Utils
{
    /// <summary>
    /// Define conditions to filter input 
    /// </summary>
    public class FlowShape<T>
    {
        /// <param name="shape">Filtering conditions.</param>
        /// <param name="handler">Callback to handle input that fits the <c>shape</c>.</param>
        public FlowShape(Func<T, bool> shape, Action<T> handler)
        {
            Shape = shape;
            Handler = handler;
        }

        public Func<T, bool> Shape { get; }
        public Action<T> Handler { get; }           
    }
}
