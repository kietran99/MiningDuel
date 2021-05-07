using UnityEngine;

namespace MD.Character
{
    public struct CounterSuccessData : EventSystems.IEventData
    {
        public Vector2 counterDir;

        public CounterSuccessData(Vector2 counterDir)
        {
            this.counterDir = counterDir;
        }
    }
}
