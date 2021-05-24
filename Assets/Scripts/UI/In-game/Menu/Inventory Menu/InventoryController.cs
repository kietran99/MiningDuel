﻿using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MD.Character;
using MD.Diggable.Projectile;

public class InventoryController : NetworkBehaviour
{
    [System.Serializable]
    public class InventoryItem
    {
        public InventoryItemType type;
        [SerializeField]
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
            return this.amount;
        }

        public int IncreaseAmount(int amount=1)
        {
            this.amount += amount;
            return this.amount;
        }
    }

    public enum InventoryItemType
    {
        PickAxe = 0,
        Trap = 1,
    }

    public MainActionType GetActionType(InventoryItemType itemType)
    {
        switch (itemType)
        {
            case InventoryItemType.PickAxe:
                return MainActionType.DIG;
            case InventoryItemType.Trap:
                return MainActionType.SETTRAP;
            default:
                return MainActionType.DIG;
        }
    }

    [SerializeField]
    private List<InventoryItem> inventory;
    [SerializeField]
    private int currentIndex = 0;
    private int count;

    void Start()
    {
        inventory = new List<InventoryItem>();
        InventoryItem pickaxe = new InventoryItem(InventoryItemType.PickAxe, 1, false);
        AddItem(pickaxe);
        var consumer = GetComponent<EventSystems.EventConsumer>();
        if (consumer == null)
        {
            consumer = gameObject.AddComponent<EventSystems.EventConsumer>();
        }
        consumer.StartListening<InventoryMenuIndexChangeData>(HandleInventoryIndexChange);
        consumer.StartListening<SetTrapInvokeData>(HandleSetTrapInvoke);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            InventoryItem trap = new InventoryItem(InventoryItemType.Trap, 3);
            AddItem(trap);
        }
    }

    private void HandleInventoryIndexChange(InventoryMenuIndexChangeData data)
    {
        if (data.index < 0 || data.index >= inventory.Count)
        {
            Debug.LogError("index out of bound");
            return;
        }
        currentIndex = data.index;
        MainActionType type = GetActionType(inventory[currentIndex].type);
        EventSystems.EventManager.Instance.TriggerEvent<MainActionToggleData>(new MainActionToggleData(type));
    }

    private void HandleSetTrapInvoke(SetTrapInvokeData data)
    {
        if (inventory[currentIndex].type != InventoryItemType.Trap)
        {
            Debug.LogError("wrong item" + inventory[currentIndex].type + " of index " + currentIndex );
            return;
        }
        if (inventory[currentIndex].amount <= 0 )
        {
            Debug.LogError("error using out of stack item");
            return;
        }
        CmdRequestSpawnLinkedTrap(netIdentity,Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        UseItem(currentIndex);
    }

    [Command]
    private void CmdRequestSpawnLinkedTrap(NetworkIdentity owner, int x, int y)
    {
        var data = new LinkedTrapSpawnData(owner,x,y);
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
            new InventoryItemAmountChangeData(index, inventory[index].amount));
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
