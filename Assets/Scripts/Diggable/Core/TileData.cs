namespace MD.Diggable.Core
{
    public class TileData
    {
        public TileData(DiggableType type)
        {
            Type = type;
            DigsLeft = type.Equals(DiggableType.Empty) ? 0 : DiggableTypeConverter.Convert(type).DigValue;
        }

        public int DigsLeft { get; private set; }

        public DiggableType Type { get; private set; }
        
        public override string ToString() => Type.ToString() + " Digs left: " + DigsLeft;
    }
}