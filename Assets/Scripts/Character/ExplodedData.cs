namespace MD.Character
{
    public struct ExplodedData : EventSystems.IEventData
    {
        public uint explodedPlayerId;
        public int dropAmount;

        public ExplodedData(uint explodedPlayerId, int dropAmount)
        {
            this.explodedPlayerId = explodedPlayerId;
            this.dropAmount = dropAmount;
        }
    }
}