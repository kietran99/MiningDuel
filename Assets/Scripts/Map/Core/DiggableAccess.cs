namespace MD.Diggable.Core
{
    public struct DiggableAccess : IDiggableAccess
    {          
        public DiggableAccess(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; private set; }

        public int Y {get ; private set; }
    }
}
