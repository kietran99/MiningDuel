using UnityEngine;

namespace MD.VisualEffects
{
    public struct ExplosionEffectRequestData : EventSystems.IEventData
    {
        public Vector2 pos;

        public ExplosionEffectRequestData(Vector2 pos)
        {
            this.pos = pos;
        }
    }
}