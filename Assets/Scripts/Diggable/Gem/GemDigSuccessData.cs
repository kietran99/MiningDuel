namespace MD.Diggable
{
    public struct GemDigSuccessData : EventSystems.IEventData
    {
        public float posX, posY;
        public int value;

        public GemDigSuccessData(float posX, float posY, int value)
        {
            this.posX = posX;
            this.posY = posY;
            this.value = value;
        }
    }
}