namespace MD.Diggable.Gem
{
    public struct GemSpawnData : EventSystems.IEventData
    {
        public float x, y;
        public DiggableType type;

        public GemSpawnData(float x, float y, DiggableType type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }
    }
}