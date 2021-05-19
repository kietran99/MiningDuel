using UnityEngine;

namespace MD.Character
{
    public struct GetCounteredData : EventSystems.IEventData
    {
        public Vector2 counterVect;
        public float immobilizeTime;

        public GetCounteredData(Vector2 counterVect, float immobilizeTime)
        {
            this.counterVect = counterVect;
            this.immobilizeTime = immobilizeTime;
        }
    }
}