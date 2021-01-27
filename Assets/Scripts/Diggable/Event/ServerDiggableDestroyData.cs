using MD.Character;

namespace MD.Diggable
{
    //data for sonar and map on client to update
    public struct ServerDiggableDestroyData : EventSystems.IEventData
    {
        public float posX;
        public float posY;
        public int diggable;
        public DigAction digger;
        public ServerDiggableDestroyData(int diggable, float posX, float posY, DigAction digger)
        {
            this.diggable = diggable;
            this.posX = posX;
            this.posY = posY;
            this.digger = digger;
        }
    }
}