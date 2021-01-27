namespace MD.Character
{
    public struct ExplodedData : EventSystems.IEventData
    {
        public uint explodedTargetID;
        public int dropAmount;

        public ExplodedData(uint explodedPlayerId, int dropAmount)
        {
            this.explodedTargetID = explodedPlayerId;
            this.dropAmount = dropAmount;
        }
    }
}