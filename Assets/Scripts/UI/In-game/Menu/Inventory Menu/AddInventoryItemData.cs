namespace MD.UI
{
    public class AddInventoryItemData: EventSystems.IEventData
    {
        public int index;

        public InventoryController.InventoryItem item;

        //number of gems used
        public AddInventoryItemData(int index, InventoryController.InventoryItem item)
        {
            this.index = index;
            this.item = item;
        }
    }
}