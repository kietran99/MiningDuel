namespace MD.Diggable.Gem
{
    public struct GemDigData : EventSystems.IEventData
    {
        public uint diggerID;
        public int value;

        public GemDigData(uint diggerID, int value)
        {
            this.diggerID = diggerID;
            this.value = value;
        }
    }
}