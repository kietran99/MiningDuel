using System.Collections.Generic;
using UnityEngine;

namespace MD.UI
{
    public class CraftingMenuUI : MonoBehaviour
    {
        [SerializeField]
        private Transform container = null;

        [SerializeField]
        private CraftItemUI itemUI = null;
        
        [SerializeField]
        private List<CraftItemUI> ItemUIObjectsList = null;
        
        [SerializeField]
        private CraftingMenuDrag menuDrag = null;

        [SerializeField]
        private bool isInThisMenu = false;

        void Start()
        {
            ItemUIObjectsList = new List<CraftItemUI>();
            var consumer = gameObject.AddComponent<EventSystems.EventConsumer>();
            consumer.StartListening<CraftableItemsListChangeData>(HandleListChange);
            consumer.StartListening<MenuSwitchEvent>(HandleMenuSwitchEvent);
        }

        void HandleListChange(CraftableItemsListChangeData data)
        {
            if (isInThisMenu)
            {
                EventSystems.EventManager.Instance.TriggerEvent(new SetCraftButtonData(data.itemsList.Count>0));
            }
            int delta = data.itemsList.Count - ItemUIObjectsList.Count;
            if (delta > 0)
            {
                for (int i = 0; i <delta; i++)
                {
                    CraftItemUI item =  Instantiate(itemUI,Vector3.zero,Quaternion.identity,container).GetComponent<CraftItemUI>();
                    ItemUIObjectsList.Add(item);
                }
                EventSystems.EventManager.Instance.TriggerEvent<CraftItemsNumberChangeData>(new CraftItemsNumberChangeData(ItemUIObjectsList.Count));
            }
            else if (delta < 0)
            {
                for (int i = 0; i < -delta; i++)
                {
                    Debug.Log("destroy gameobject");
                    Destroy(ItemUIObjectsList[0].gameObject);
                    ItemUIObjectsList.RemoveAt(0);
                }
                EventSystems.EventManager.Instance.TriggerEvent<CraftItemsNumberChangeData>(new CraftItemsNumberChangeData(ItemUIObjectsList.Count));
            }
            for (int i =0; i <ItemUIObjectsList.Count; i++)
            {
                Debug.Log("list count" + ItemUIObjectsList.Count + "  new list count" + data.itemsList.Count);
                // Debug.Log("new list " + data.itemsList);
                if (ItemUIObjectsList[i].Name() == data.itemsList[i]) continue;
                ItemUIObjectsList[i].SetItem(data.itemsList[i]);
                menuDrag.TriggerIndexChangeEvent();
            }
        }

        private void HandleMenuSwitchEvent(MenuSwitchEvent data)
        {
            Debug.Log("received data switchtoinventory " + data.switchToInventoryMenu);
            isInThisMenu = !data.switchToInventoryMenu;
            if (isInThisMenu && ItemUIObjectsList.Count>0)
            {
                EventSystems.EventManager.Instance.TriggerEvent(new SetCraftButtonData(true));
            }
        }

    }
}
