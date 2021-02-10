namespace MD.Diggable.Core
{
    public readonly struct SonarTileData : System.IEquatable<SonarTileData>
    {
        public readonly int x, y;
        public readonly DiggableType type;

        public SonarTileData(int x, int y, DiggableType type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }

        public bool Equals(SonarTileData other)
        {
            return other.x == x && other.y == y;
        }

        public override bool Equals(object other)
        {
            if (other == null || !GetType().Equals(other.GetType()))
            {
                return false;
            }

            var castedOther = (SonarTileData) other;
            return castedOther.x == x && castedOther.y == y;
        }

        public override int GetHashCode()
        {
            return (x << 2) & y;
        }
    }
}