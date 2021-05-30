using UnityEngine;

namespace MD.Character
{
    public struct AttackTargetChangeData : EventSystems.IEventData
    {
        public readonly bool attackable;
        public readonly UnityEngine.Vector2 targetPos;

        public AttackTargetChangeData(bool attackable, Vector2 targetPos)
        {
            this.attackable = attackable;
            this.targetPos = targetPos;
        }
    }
}