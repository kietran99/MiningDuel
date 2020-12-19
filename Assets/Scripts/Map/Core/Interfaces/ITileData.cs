namespace MD.Map.Core
{
    public interface ITileData
    {
        int DigsLeft { get; }
        DiggableType Type { get; }


        /// <summary>
        /// Reduce remaning digs count by <param name="reduceVal">reduceVal</param> 
        /// </summary>
        void Reduce(int reduceVal, out bool isEmpty);
        bool IsEmpty();
    }
}
