namespace MD.Diggable.Core
{
    public class NullTileData : TileData
    {
        public NullTileData()
        {
            Type = DiggableType.Empty;
            DigsLeft = 0;
        }
    }
}