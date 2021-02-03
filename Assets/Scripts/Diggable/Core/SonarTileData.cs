namespace MD.Diggable.Core
{
    public struct SonarTileData
    {
        public int x, y;
        public DiggableType type;

        public SonarTileData(int x, int y, DiggableType type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }
    }
}