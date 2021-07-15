namespace MD.Character
{
    public readonly struct AttackCooldownData : EventSystems.IEventData
    {
        public readonly bool attackable;

        public AttackCooldownData(bool attackable)
        {
            this.attackable = attackable;
        }
    }
}