namespace MD.Diggable
{
    public struct DiggableRemoveData : EventSystems.IEventData
    {
        public int x, y;

        public DiggableRemoveData(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}