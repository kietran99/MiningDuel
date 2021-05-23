using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class InventoryController : NetworkBehaviour
{
    public struct InventoryItem
    {
        public InventoryItemType type;
        public int amount;

        public bool removable;

        public InventoryItem( InventoryItemType type, int amount, bool removable = true)
        {
            this.type = type;
            this.amount = amount;
            this.removable = removable;
        }

        public int ReduceAmount(int amount=1)
        {
            this.amount -= amount;
            return amount;
        }

        public int IncreaseAmount(int amount=1)
        {
            this.amount += amount;
            return amount;
        }
    }

    public enum InventoryItemType
    {
        PickAxe = 0,
        Trap = 1,
    }

    [SerializeField]
    private List<InventoryItem> inventory;
    private int count;

    void Start()
    {
        inventory = new List<InventoryItem>();
        InventoryItem pickaxe = new InventoryItem(InventoryItemType.PickAxe, 1, false);
        AddItem(pickaxe);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            InventoryItem trap = new InventoryItem(InventoryItemType.Trap, 3);
            AddItem(trap);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            CmdRequestSpawnLinkedTrap(netIdentity,Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
            UseItem(1);
        }
    }

    [Command]
    private void CmdRequestSpawnLinkedTrap(NetworkIdentity owner, int x, int y)
    {
            LinkedTrapSpawnData data = new LinkedTrapSpawnData(owner,x,y);
            EventSystems.EventManager.Instance.TriggerEvent<LinkedTrapSpawnData>(data);
    }

    public bool UseItem(int index) //return true if remove an item
    {
        if (index <0 || index >= inventory.Count)
        {
            Debug.Log("index out of bound");
            return false;
        }
        if (!inventory[index].removable) return false;
        if (inventory[index].ReduceAmount() <= 0)
        {
            inventory.RemoveAt(index);
            EventSystems.EventManager.Instance.TriggerEvent<RemoveInventoryItemData>(
                new RemoveInventoryItemData(index));
            return true;
        }
        EventSystems.EventManager.Instance.TriggerEvent<InventoryItemAmountChangeData>(
            new InventoryItemAmountChangeData(index, inventory.Count));
        return false;
    }

    public bool AddItem(InventoryItem item) //return true if add new item
    {
        for (int i=1; i< inventory.Count ; i++)
        {
            if (item.type.Equals(inventory[i].type))
            {
                inventory[i].IncreaseAmount(item.amount);
                EventSystems.EventManager.Instance.TriggerEvent<InventoryItemAmountChangeData>(
                    new InventoryItemAmountChangeData(i, inventory[i].amount));
                return false;
            }
        }
        inventory.Add(item);
        EventSystems.EventManager.Instance.TriggerEvent<AddInventoryItemData>
        (new AddInventoryItemData(inventory.Count -1, item));
        return true;
    }

}
