namespace MD.Diggable.Core
{
    public interface IMap
    {
        bool TryGet(int x, int y, out TileData data);
        bool TrySet(int x, int y, in TileData data);
        void Log();
    }
}