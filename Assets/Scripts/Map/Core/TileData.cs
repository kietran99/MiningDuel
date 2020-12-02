using MD.Diggable.Core;

namespace MD.Map.Core
{
    public class TileData : ITileData
    {
        public static TileData Empty { get => empty; }
        private static TileData empty = new TileData(DiggableType.Empty);

        public TileData(DiggableType type)
        {
            Type = type;
            DigsLeft = type.Equals(DiggableType.Empty) ? 0 : DiggableTypeConverter.Convert(type).DigValue;
        }

        public int DigsLeft { get; protected set; }

        public DiggableType Type { get; protected set; }
        
        public void Reduce(int reduceVal)
        {
            DigsLeft -= reduceVal;
            DigsLeft = DigsLeft > 0 ? DigsLeft : 0;
        }

        public bool IsEmpty() => DigsLeft == 0;

        public override string ToString() => Type.ToString() + " Digs left: " + DigsLeft;
    }
}