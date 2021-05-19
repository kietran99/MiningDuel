using UnityEngine;

namespace MD.Character
{
    public struct DamageGivenData : EventSystems.IEventData
    {
        public Vector2 damagablePos;
        public int dmg;

        public DamageGivenData(Vector2 damagablePos, int dmg)
        {
            this.damagablePos = damagablePos;
            this.dmg = dmg;
        }
    }
}
