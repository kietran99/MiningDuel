namespace MD.Diggable.Gem
{
    public struct GemDugData : EventSystems.IEventData
    {
        public uint diggerID;
        public int value;

        public GemDugData(uint diggerID, int value)
        {
            this.diggerID = diggerID;
            this.value = value;
        }
    }
}