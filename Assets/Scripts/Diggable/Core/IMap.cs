namespace MD.Map.Core
{
    public interface IMap
    {
        bool TryGetAt(int x, int y, out TileData data);
        bool TrySetAt(int x, int y, in TileData data);
        bool TryReduceAt(int x, int y, int reduceVal);
        bool IsEmptyAt(int x, int y);
        void Log();
    }
}