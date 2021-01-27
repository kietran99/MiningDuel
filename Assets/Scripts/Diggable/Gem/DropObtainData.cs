namespace MD.Diggable.Gem
{
    public struct DropObtainData : EventSystems.IEventData
    {
        public uint pickerID;
        public int value;

        public DropObtainData(uint pickerID, int value)
        {
            this.pickerID = pickerID;
            this.value = value;
        }
    }
}