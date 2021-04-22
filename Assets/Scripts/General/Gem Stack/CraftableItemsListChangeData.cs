using MD.CraftingSystem;
using System.Collections.Generic;
public class CraftableItemsListChangeData : EventSystems.IEventData
{
    public List<CraftItemName> itemsList;
    public CraftableItemsListChangeData(List<CraftItemName> itemsList) => this.itemsList = itemsList;
}