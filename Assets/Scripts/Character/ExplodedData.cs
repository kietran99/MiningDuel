namespace MD.Character
{
    public struct ExplodedData : EventSystems.IEventData
    {
        public uint explodedTargetID;
        public int dropAmount;
        public float immobilizeTime;

        public ExplodedData(uint explodedTargetID, int dropAmount, float immobilizeTime)
        {
            this.explodedTargetID = explodedTargetID;
            this.dropAmount = dropAmount;
            this.immobilizeTime = immobilizeTime;
        }
    }
}