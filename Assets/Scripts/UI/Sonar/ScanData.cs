namespace MD.UI
{
    public struct ScanData : EventSystems.IEventData 
    { 
        public DiggableType[] diggableArea;

        public ScanData(DiggableType[] diggableArea)
        {
            this.diggableArea = diggableArea;
        }
    }
}