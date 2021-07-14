public struct CraftItemsNumberChangeData: EventSystems.IEventData
{
    //index of the first used gem start from bottom of stack
    public int numOfItems;
    //number of gems used
    public CraftItemsNumberChangeData(int numOfItems)
    {
        this.numOfItems = numOfItems;
    }
}