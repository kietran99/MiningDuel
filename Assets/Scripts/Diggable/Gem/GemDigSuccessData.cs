using MD.Character;
namespace MD.Diggable
{
    public struct GemDigSuccessData : EventSystems.IEventData
    {
        public float posX, posY;
        public int value;

        public GemDigSuccessData(int value, float posX, float posY)
        {
            this.posX = posX;
            this.posY = posY;
            this.value = value;
        }
    }
}