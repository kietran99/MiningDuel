using UnityEngine;

namespace MD.Character
{
    public struct DamageGivenData : EventSystems.IEventData
    {
        public Vector2 damagablePos;
        public int dmg;
        public bool isCritical;

        public DamageGivenData(Vector2 damagablePos, int dmg, bool isCritical)
        {
            this.damagablePos = damagablePos;
            this.dmg = dmg;
            this.isCritical = isCritical;
        }
    }
}
