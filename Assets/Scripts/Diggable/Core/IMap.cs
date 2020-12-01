namespace MD.Map.Core
{
    /// <summary>
    /// Manages in-game map and tile data
    /// </summary>
    public interface IMap
    {
        bool TryGetAt(int x, int y, out ITileData data);
        bool TrySetAt(int x, int y, ITileData data);
        bool TryReduceAt(int x, int y, int reduceVal);
        /// <exception cref="MD.Map.Core.InvalidTileException">H</exception>
        void SetAt(int x, int y, ITileData data);
        /// <exception cref="MD.Map.Core.InvalidTileException">J</exception>
        void ReduceAt(int x, int y, int reduceVal);
        /// <exception cref="MD.Map.Core.InvalidTileException">J</exception>
        bool IsEmptyAt(int x, int y);       
        void Log();
    }
}