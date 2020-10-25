namespace MD.Diggable.Core
{
    public class TileData
    {
        public TileData() { }

        public TileData(DiggableType type)
        {
            Type = type;
            DigsLeft = type.Equals(DiggableType.Empty) ? 0 : DiggableTypeConverter.Convert(type).DigValue;
        }

        public int DigsLeft { get; protected set; }

        public DiggableType Type { get; protected set; }
        
        public override string ToString() => Type.ToString() + " Digs left: " + DigsLeft;
    }
}