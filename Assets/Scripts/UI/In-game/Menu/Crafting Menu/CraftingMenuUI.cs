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

        void Start()
        {
            ItemUIObjectsList = new List<CraftItemUI>();
            gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<CraftableItemsListChangeData>(HandleListChange);
        }

        void HandleListChange(CraftableItemsListChangeData data)
        {
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

    }
}
