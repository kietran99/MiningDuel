using Functional.Type;
using UnityEngine;

namespace MD.Map.Core
{
    public class DiggableAccess : IDiggableAccess
    {          
        public DiggableAccess(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }

        public int Y {get ; private set; }

        // public static Option<IDiggableAccess> Create(IDiggableData data)
        // {
        //     if (data == null)
        //     {
        //         Debug.LogError("Must pass an IDiggableData");
        //         return new Option<IDiggableAccess>();
        //     }

        //     return new DiggableAccess();
        // }
    }
}
