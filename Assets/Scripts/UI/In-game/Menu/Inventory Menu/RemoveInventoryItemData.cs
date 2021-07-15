public struct RemoveInventoryItemData: EventSystems.IEventData
{

    public int index;

    //number of gems used
    public RemoveInventoryItemData(int index)
    {
        this.index = index;
    }
}