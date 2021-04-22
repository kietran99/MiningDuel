namespace MD.Diggable.Gem
{
    public struct GemObtainData : EventSystems.IEventData
    {
        public uint diggerID;
        public int value;

        public DiggableType type;

        public GemObtainData(uint diggerID, int value, DiggableType type)
        {
            this.diggerID = diggerID;
            this.value = value;
            this.type = type;
        }
    }
}