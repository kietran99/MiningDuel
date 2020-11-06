using MD.Character;
namespace MD.Diggable
{
    public struct GemDigSuccessData : EventSystems.IEventData
    {
        public float posX, posY;
        public int value;

        public DigAction digger;

        public GemDigSuccessData(float posX, float posY, int value, DigAction digger)
        {
            this.posX = posX;
            this.posY = posY;
            this.value = value;
            this.digger = digger;
        }
    }
}