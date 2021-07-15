using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MD.Character;
using MD.Diggable.Projectile;

namespace MD.UI
{
    public class InventoryController : NetworkBehaviour
    {
        [System.Serializable]
        public class InventoryItem
        {
            public InventoryItemType type;

            public int amount;

            public bool removable;

            public InventoryItem(InventoryItemType type, int amount, bool removable = true)
            {
                this.type = type;
                this.amount = amount;
                this.removable = removable;
            }

            public int ReduceAmount(int amount = 1)
            {
                this.amount -= amount;
                return this.amount;
            }

            public int IncreaseAmount(int amount = 1)
            {
                this.amount += amount;
                return this.amount;
            }
        }

        public enum InventoryItemType
        {
            PickAxe = 0,
            Trap = 1,
            CamoTrap = 2
        }

        public MainActionType GetActionType(InventoryItemType itemType)
        {
            switch (itemType)
            {
                case InventoryItemType.PickAxe:
                    return MainActionType.DIG;
                case InventoryItemType.Trap:
                    return MainActionType.SETTRAP;
                case InventoryItemType.CamoTrap:
                    return MainActionType.SETTRAP;
                default:
                    return MainActionType.DIG;
            }
        }

        private List<InventoryItem> inventory;
        private int currentIndex = 0;

        public override void OnStartAuthority()
        {
            inventory = new List<InventoryItem>();
            InventoryItem pickaxe = new InventoryItem(InventoryItemType.PickAxe, 1, false);
            AddItem(pickaxe);
            var eventConsumer = EventSystems.EventConsumer.GetOrAttach(gameObject);
            eventConsumer.StartListening<InventoryMenuIndexChangeData>(HandleInventoryIndexChange);
            eventConsumer.StartListening<SetTrapInvokeData>(HandleSetTrapInvoke);
        }

        private void HandleInventoryIndexChange(InventoryMenuIndexChangeData data)
        {
            if (data.index < 0 || data.index >= inventory.Count)
            {
                Debug.LogError("index out of bound index: " + data.index + " count: " + inventory.Count);
                return;
            }

            currentIndex = data.index;

            var isMainWeapon = inventory[currentIndex].type.Equals(InventoryItemType.PickAxe);
            EventSystems.EventManager.Instance.TriggerEvent(new MainWeaponToggleData(isMainWeapon));

            if (!isMainWeapon)
            {
                MainActionType type = GetActionType(inventory[currentIndex].type);
                EventSystems.EventManager.Instance.TriggerEvent(new MainActionToggleData(type));
            }
        }

        public bool UseItem(int index) //return true if remove an item
        {
            if (index < 0 || index >= inventory.Count)
            {
                Debug.Log("index out of bound");
                return false;
            }

            if (!inventory[index].removable) 
            {
                return false;
            }

            if (inventory[index].ReduceAmount() <= 0)
            {
                inventory.RemoveAt(index);
                EventSystems.EventManager.Instance.TriggerEvent(new RemoveInventoryItemData(index));
                return true;
            }

            EventSystems.EventManager.Instance.TriggerEvent(new InventoryItemAmountChangeData(index, inventory[index].amount));
            return false;
        }

        public bool AddItem(InventoryItem item) //return true if add new item
        {
            for (int i = 1; i < inventory.Count; i++)
            {
                if (item.type.Equals(inventory[i].type))
                {
                    inventory[i].IncreaseAmount(item.amount);
                    EventSystems.EventManager.Instance.TriggerEvent(new InventoryItemAmountChangeData(i, inventory[i].amount));
                    return false;
                }
            }

            inventory.Add(item);
            EventSystems.EventManager.Instance.TriggerEvent(new AddInventoryItemData(inventory.Count -1, item));
            return true;
        }

        private void HandleSetTrapInvoke(SetTrapInvokeData data)
        {
            if (inventory[currentIndex].amount <= 0 )
            {
                Debug.LogError("error using out of stack item");
                return;
            }

            if (inventory[currentIndex].type == InventoryItemType.Trap)
            {
                CmdRequestSpawnLinkedTrap(netIdentity, Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
            } 
            else if (inventory[currentIndex].type == InventoryItemType.CamoTrap)
            {
                CmdRequestSpawnCamoTrap(netIdentity, Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
            } 
            else
            {
                Debug.Log("error type : " + inventory[currentIndex].type);
                return;
            }

            UseItem(currentIndex);
        }

        [TargetRpc]
        public void TargetObtainTraps(NetworkConnection conn)
        {
            InventoryController.InventoryItem trap = new InventoryController.InventoryItem(InventoryController.InventoryItemType.Trap, 3);
            AddItem(trap);
        }

        public void ObtainCamoPerse()
        {
            InventoryController.InventoryItem camoTrap = new InventoryController.InventoryItem(InventoryItemType.CamoTrap, 3);
            AddItem(camoTrap);
        }
        
        [Command]
        private void CmdRequestSpawnLinkedTrap(NetworkIdentity owner, int x, int y)
        {
            var data = new LinkedTrapSpawnData(owner,x,y);
            EventSystems.EventManager.Instance.TriggerEvent<LinkedTrapSpawnData>(data);
        }

        [Command]
        private void CmdRequestSpawnCamoTrap(NetworkIdentity owner, int x, int y)
        {
            var plantable = ServiceLocator
                .Resolve<Diggable.Core.IDiggableGenerator>()
                .Match(err => false, digGen => digGen.IsEmptyAt(x, y).Match(err => false, isEmpty => isEmpty));

            if (!plantable)
            {
                TargetReturnCamo();
                return;
            }

            var data = new CamoPerseSpawnData(owner, x, y);
            EventSystems.EventManager.Instance.TriggerEvent<CamoPerseSpawnData>(data);
        }

        [TargetRpc]
        private void TargetReturnCamo() => AddItem(new InventoryController.InventoryItem(InventoryItemType.CamoTrap, 1));
    }
}
