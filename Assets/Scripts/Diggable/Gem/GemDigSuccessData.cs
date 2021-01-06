namespace MD.Diggable
{
    public struct GemDigSuccessData : EventSystems.IEventData
    {
        public uint diggerID;
        public float posX, posY;
        public int value;

        public GemDigSuccessData(uint diggerID, float posX, float posY, int value)
        {
            this.diggerID = diggerID;
            this.posX = posX;
            this.posY = posY;
            this.value = value;
        }
    }
}