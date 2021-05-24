public struct InventoryMenuIndexChangeData: EventSystems.IEventData
{
    public int index;
    public InventoryMenuIndexChangeData(int index)
    {
        this.index = index;
    }
}