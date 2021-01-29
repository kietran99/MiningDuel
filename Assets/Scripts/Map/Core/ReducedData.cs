namespace MD.Map.Core
{
    public struct ReducedData
    {
        public int current, max;
        public DiggableType type;

        public bool isEmpty { get => current <= 0; }

        public ReducedData(DiggableType type, int current, int max)
        {
            this.type = type;
            this.current = current;
            this.max = max;
        }
    }
}