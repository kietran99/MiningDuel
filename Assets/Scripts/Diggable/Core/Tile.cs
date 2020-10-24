namespace MD.Diggable.Core
{
    public class Tile
    {
        public int X { get; }
        public int Y { get; }
        public TileData Data { get; set; }

        public Tile(int x, int y, TileData data)
        {
            X = x;
            Y = y;
            Data = data;
        }

        public override string ToString()
        {
            return "[" + X + ", " + Y + "] " + Data.ToString();
        }
    }
}