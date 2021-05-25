using UnityEngine;
using MD.CraftingSystem;
using Mirror;
using MD.Quirk;

public class CraftingManagerServer : NetworkBehaviour
{
    [SerializeField]
    private CraftingRecipe recipeSO;

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
        BaseQuirk itemPrefab = GetItem(data.item);
        if (itemPrefab == null) return;
        BaseQuirk ins = Instantiate(itemPrefab.gameObject,Vector3.zero, Quaternion.identity, data.player.transform).GetComponent<BaseQuirk>();
        NetworkServer.Spawn(ins.gameObject,data.player.connectionToClient);
        ins.Activate(data.player);
    }

    BaseQuirk GetItem(CraftItemName item)
    {
        return recipeSO.GetItem(item);
    }
}
