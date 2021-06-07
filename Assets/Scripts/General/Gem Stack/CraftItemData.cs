using Mirror;

namespace MD.CraftingSystem
{
    public class CraftItemData : EventSystems.IEventData
    {
        public NetworkIdentity player;
        public CraftItemName item;
        
        public CraftItemData(NetworkIdentity player, CraftItemName item)
        {
            this.player = player;
            this.item = item;
        }
    }
}