namespace MD.UI
{
    public struct QuirkInvokeData : EventSystems.IEventData
    {
        public int idx;

        public QuirkInvokeData(int idx)
        {
            this.idx = idx;
        }
    } 
}