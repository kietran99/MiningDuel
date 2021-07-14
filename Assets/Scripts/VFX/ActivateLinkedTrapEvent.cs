namespace MD.Diggable.Projectile
{
    public struct ActivateLinkedTrapEvent : EventSystems.IEventData
    {
        public uint activatorId;

        public ActivateLinkedTrapEvent(uint activatorId)
        {
            this.activatorId = activatorId;
        }
    }
}
