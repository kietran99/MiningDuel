namespace MD.Diggable.Core
{
    public interface ITileData
    {
        int DigsLeft { get; }
        DiggableType Type { get; }
        bool IsEmpty { get; }

        /// <summary>
        /// Reduce remaning digs count by <param name="reduceVal">reduceVal</param> 
        /// </summary>
        ReducedData Reduce(int reduceVal);
    }
}
