namespace MD.UI
{
    public struct InventoryItemAmountChangeData: EventSystems.IEventData
    {
        //index of the first used gem start from bottom of stack
        public int index;
        public int amount;
        //number of gems used
        public InventoryItemAmountChangeData(int index, int amount)
        {
            this.index = index;
            this.amount = amount;
        }
    }
}