using UnityEngine;

namespace MD.AI
{
    public struct BotCounterSuccessData : EventSystems.IEventData
    {
        public Vector2 counterDir;

        public BotCounterSuccessData(Vector2 counterDir)
        {
            this.counterDir = counterDir;
        }
    }
}