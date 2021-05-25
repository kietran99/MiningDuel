using UnityEngine;

namespace MD.Character
{
    public class AttackDirectionData : EventSystems.IEventData
    {
        public Vector2 dir;

        public AttackDirectionData(Vector2 dir)
        {
            this.dir = dir;
        }
    }
}
