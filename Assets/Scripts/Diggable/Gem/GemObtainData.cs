namespace MD.Diggable.Gem
{
    public struct GemObtainData : EventSystems.IEventData
    {
        public uint diggerID;
        public int value;

        public GemObtainData(uint diggerID, int value)
        {
            this.diggerID = diggerID;
            this.value = value;
        }
    }
}