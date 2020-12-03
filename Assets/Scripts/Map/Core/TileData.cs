using MD.Diggable.Core;

namespace MD.Map.Core
{
    public class TileData : ITileData
    {
        public static TileData Empty { get => empty; }
        private static TileData empty = new TileData(DiggableType.Empty);

        private int digsLeft;

        public TileData(DiggableType type)
        {
            Type = type;
            DigsLeft = type.Equals(DiggableType.Empty) ? 0 : DiggableTypeConverter.Convert(type).DigValue;
        }

        public int DigsLeft 
        { 
            get => digsLeft; 
            private set
            {
                if (value > 0)
                {
                    digsLeft = value;
                    return;
                }

                digsLeft = 0;
                Type = DiggableType.Empty;
            } 
        }

        public DiggableType Type { get; protected set; }
        
        public bool Reduce(int reduceVal)
        {
            DigsLeft -= reduceVal;
            DigsLeft = DigsLeft > 0 ? DigsLeft : 0;
            return IsEmpty();
        }

        public bool IsEmpty() => DigsLeft == 0;

        public override string ToString() => "   |   " + Type.ToString() + "    |     Digs left: " + DigsLeft;
    }
}