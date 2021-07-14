using UnityEngine;

namespace MD.AI
{
    public struct BotGetCounteredData : EventSystems.IEventData
    {
        public Vector2 counterVect;
        public float immobilizeTime;

        public BotGetCounteredData(Vector2 counterVect, float immobilizeTime)
        {
            this.counterVect = counterVect;
            this.immobilizeTime = immobilizeTime;
        }
    }
}