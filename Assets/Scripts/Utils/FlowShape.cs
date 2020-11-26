using System;

namespace Utils
{
    public class FlowShape<T>
    {
        public FlowShape(Func<T, bool> canApply, Action<T> transform)
        {
            CanApply = canApply;
            Transform = transform;
        }

        public Func<T, bool> CanApply { get; }
        public Action<T> Transform { get; }           
    }

    // public class FlowTransform<T1, T2>
    // {
    //     public FlowTransform(Func<T1, T2, bool> canApply, Action<T1, T2> transform)
    //     {
    //         CanApply = canApply;
    //         Transform = transform;
    //     }

    //     public Func<T1, T2, bool> CanApply { get; }
    //     public Action<T1, T2> Transform { get; }           
    // }
}
