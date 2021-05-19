namespace MD.Character
{
    public struct StunData : EventSystems.IEventData
    {
        public bool isStunned;

        public StunData(bool isStunned)
        {
            this.isStunned = isStunned;
        }
    }
}