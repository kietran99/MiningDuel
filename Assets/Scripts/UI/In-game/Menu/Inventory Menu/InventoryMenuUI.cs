using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InventoryMenuUI : MonoBehaviour
{
    private int count = 0;
    [SerializeField]
    private Transform SpawnContainer = null;
    [SerializeField]
    private GameObject PickAxeItem = null;
    [SerializeField]
    private GameObject TrapItem = null;

    List<GameObject> inventoryUI;

    void Awake()
    {
        inventoryUI = new List<GameObject>();
        var eventConsumer = gameObject.AddComponent<EventSystems.EventConsumer>();
        eventConsumer.StartListening<AddInventoryItemData>(HandleAddItem);
        eventConsumer.StartListening<RemoveInventoryItemData>(HandleRemoveItem);
        eventConsumer.StartListening<InventoryItemAmountChangeData>(HandleItemAmountChange);
    }

    void Start()
    {
        //since player spawn before enter mulitplayer scene
        //trigger an fake event aquiring pickaxe when enter multiplayer scene for UI to update
        InventoryController.InventoryItem pickAxe = new InventoryController.InventoryItem(InventoryController.InventoryItemType.PickAxe,1,false);
        EventSystems.EventManager.Instance.TriggerEvent<AddInventoryItemData>
        (new AddInventoryItemData(0, pickAxe));
    }


    private void HandleAddItem(AddInventoryItemData data)
    {
        GameObject prefab = GetPrefab(data.item.type);
        if (prefab.Equals(null) ) return;
        GameObject item =  Instantiate(prefab,Vector3.zero,Quaternion.identity,SpawnContainer);
        inventoryUI.Add(item);
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
        GameObject obj = inventoryUI[data.index]; 
        inventoryUI.RemoveAt(data.index);
        Destroy(obj);
    }

    private void HandleItemAmountChange(InventoryItemAmountChangeData data)
    {

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
