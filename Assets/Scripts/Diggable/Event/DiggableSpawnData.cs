namespace MD.Diggable
{
    public struct DiggableSpawnData : EventSystems.IEventData
    {
        public int x, y;
        public DiggableType type;

        public DiggableSpawnData(int x, int y, DiggableType type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }
    }
}