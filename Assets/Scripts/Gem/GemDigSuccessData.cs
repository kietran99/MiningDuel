namespace MD.Diggable
{
    public struct GemDigSuccessData : EventSystems.IEventData
    {
        public int value;

        public GemDigSuccessData(int value)
        {
            this.value = value;
        }
    }
}