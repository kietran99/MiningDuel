using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MD.UI
{
    public class InventoryMenuUI : MonoBehaviour
    {
        private int count = 0;
        [SerializeField]
        private Transform SpawnContainer = null;
        [SerializeField]
        private GameObject PickAxeItem = null;
        [SerializeField]
        private GameObject TrapItem = null;

        List<InventoryItemUIController> inventoryUI;

        void Awake()
        {
            inventoryUI = new List<InventoryItemUIController>();
            var eventConsumer = gameObject.AddComponent<EventSystems.EventConsumer>();
            eventConsumer.StartListening<AddInventoryItemData>(HandleAddItem);
            eventConsumer.StartListening<RemoveInventoryItemData>(HandleRemoveItem);
            eventConsumer.StartListening<InventoryItemAmountChangeData>(HandleItemAmountChange);
        }

        void Start()
        {
            //since player spawn and aquired a pickaxe before enter mulitplayer scene
            //trigger a fake event aquiring a pickaxe when enter multiplayer scene for UI to update
            InventoryController.InventoryItem pickAxe = new InventoryController.InventoryItem(InventoryController.InventoryItemType.PickAxe,1,false);
            EventSystems.EventManager.Instance.TriggerEvent(new AddInventoryItemData(0, pickAxe));
        }


        private void HandleAddItem(AddInventoryItemData data)
        {
            GameObject prefab = GetPrefab(data.item.type);
            if (prefab.Equals(null) ) return;
            InventoryItemUIController item =  Instantiate(prefab,Vector3.zero,Quaternion.identity,SpawnContainer).GetComponent<InventoryItemUIController>();
            inventoryUI.Add(item);
            item.SetAmount(data.item.amount);
            count++;

            if (inventoryUI.Count -1  != data.index)
            {
                Debug.LogError("something wrong here");
            }
        }

        private void HandleRemoveItem(RemoveInventoryItemData data)
        {
            if (data.index < 0 || data.index >= inventoryUI.Count )
            {
                Debug.LogError("something wrong here");            
                return;
            }
            GameObject obj = inventoryUI[data.index].gameObject; 
            inventoryUI.RemoveAt(data.index);
            Destroy(obj);
            count--;
        }

        private void HandleItemAmountChange(InventoryItemAmountChangeData data)
        {
            inventoryUI[data.index].SetAmount(data.amount);
        }

        private GameObject GetPrefab(InventoryController.InventoryItemType itemType)
        {
            switch (itemType)
            {
                case InventoryController.InventoryItemType.PickAxe:
                    return PickAxeItem;
                case InventoryController.InventoryItemType.Trap:
                    return TrapItem;
                default:
                    return null;
            }
        }
    }
}
