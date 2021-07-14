using UnityEngine;
using Mirror;
using MD.Quirk;

namespace MD.CraftingSystem
{
    public class CraftingManagerServer : NetworkBehaviour
    {
        [SerializeField]
        private CraftingRecipe recipeSO = null;

        [ServerCallback]
        private void Start()
        {
            EventSystems.EventManager.Instance.StartListening<CraftItemData>(CraftItem);
        }

        [ServerCallback]
        private void OnDisable()
        {
            EventSystems.EventManager.Instance.StopListening<CraftItemData>(CraftItem);
        }

        private void CraftItem(CraftItemData data)
        {
            BaseQuirk itemPrefab = recipeSO.GetItem(data.item);

            if (itemPrefab == null) 
            {
                return;
            }

            BaseQuirk ins = Instantiate(itemPrefab.gameObject, Vector3.zero, Quaternion.identity, data.player.transform).GetComponent<BaseQuirk>();
            NetworkServer.Spawn(ins.gameObject, data.player.connectionToClient);
            ins.ServerActivate(data.player);
        }
    }
}
