using UnityEngine;

namespace MD.Character
{
    public struct DamageTakenData : EventSystems.IEventData
    {
        public uint damagedId;
        public Vector2 atkDir;

        public DamageTakenData(uint damagedId, Vector2 atkDir)
        {
            this.damagedId = damagedId;
            this.atkDir = atkDir;
        }
    }
}