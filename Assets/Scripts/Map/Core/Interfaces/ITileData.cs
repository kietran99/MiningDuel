namespace MD.Map.Core
{
    public interface ITileData
    {
        int DigsLeft { get; }
        DiggableType Type { get; }


        /// <returns>
        /// Whether this tile is empty 
        /// </returns>
        bool Reduce(int reduceVal);
        bool IsEmpty();
    }
}
