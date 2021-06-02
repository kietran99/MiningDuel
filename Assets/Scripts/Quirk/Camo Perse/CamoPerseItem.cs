using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD.Quirk;
using Mirror;

public class CamoPerseItem : BaseQuirk
{
    public override void SingleActivate(NetworkIdentity user)
    {
        base.SingleActivate(user);
        InventoryController inv = user.GetComponent<InventoryController>();
        if (inv == null)
        {
            Debug.LogError("inventory controller component not found on player " + user.gameObject.name);
            return;
        }
        inv.ObtainCamoPerse();
    }
}
