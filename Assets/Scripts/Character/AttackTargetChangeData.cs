using UnityEngine;

namespace MD.Character
{
    public struct AttackTargetChangeData : EventSystems.IEventData
    {
        public readonly int playerId;
        public readonly bool attackable;
        public readonly UnityEngine.Vector2 targetPos;

        public AttackTargetChangeData(int playerId, bool attackable, Vector2 targetPos)
        {
            this.playerId = playerId;
            this.attackable = attackable;
            this.targetPos = targetPos;
        }
    }
}