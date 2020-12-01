namespace MD.Map.Core
{
    public interface ITileData
    {
        int DigsLeft { get; }
        DiggableType Type { get; }
        void Reduce(int reduceVal);
        bool IsEmpty();
    }
}
