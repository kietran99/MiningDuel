namespace MD.Character
{
    public struct AliveCountChangeData: EventSystems.IEventData
    {
        public int nAlive;

        public AliveCountChangeData(int nAlive)
        {
            this.nAlive = nAlive;
        }
    }
}