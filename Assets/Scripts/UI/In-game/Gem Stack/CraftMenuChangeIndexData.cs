public struct CraftMenuChangeIndexData: EventSystems.IEventData
{
    public int index;
    public CraftMenuChangeIndexData(int index)
    {
        this.index = index;
    }
}