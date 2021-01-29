using MD.Diggable.Core;

namespace MD.Map.Core
{
    public class TileData : ITileData
    {
        public static TileData Empty { get => empty; }
        private static TileData empty = new TileData(DiggableType.Empty);

        private int digsLeft, initialDigsLeft;

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

        public TileData(DiggableType type)
        {
            Type = type;
            initialDigsLeft = type.Equals(DiggableType.Empty) ? 0 : DiggableTypeConverter.Convert(type).DigValue;
            DigsLeft = initialDigsLeft;
        }

        public bool IsEmpty { get => DigsLeft == 0; }

        public DiggableType Type { get; protected set; }
        
        public ReducedData Reduce(int value)
        {
            var reducedVal = DigsLeft - value;
            var preReduceType = Type;
            DigsLeft = reducedVal > 0 ? reducedVal : 0;
            return new ReducedData(preReduceType, DigsLeft, initialDigsLeft);     
        }

        public override string ToString() => "   |   " + Type.ToString() + "    |     Digs left: " + DigsLeft;
    }
}