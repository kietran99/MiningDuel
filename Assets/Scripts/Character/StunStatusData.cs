namespace MD.Character
{
    public struct StunStatusData : EventSystems.IEventData
    {
        public bool isStunned;

        public StunStatusData(bool isStunned)
        {
            this.isStunned = isStunned;
        }
    }
}