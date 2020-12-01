namespace MD.Diggable
{
    //data for sonar and map on client to update
    public struct DiggableSpawnData : EventSystems.IEventData
    {
        public float posX;
        public float posY;
        public int diggable;
        public DiggableSpawnData(int diggable,float posX, float posY)
        {
            this.diggable= diggable;
            this.posX = posX;
            this.posY = posY;
        }
    }
}