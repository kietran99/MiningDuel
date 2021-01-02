namespace MD.Diggable.Gem
{
    public struct DigProgressData : EventSystems.IEventData
    {
        public int current, max;

        public DigProgressData(int current, int max)
        {
            this.current = current;
            this.max = max;
        }
    }
}
