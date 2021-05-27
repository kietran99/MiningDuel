using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD.Quirk;
using MD.Character;
using Mirror;

public class Syringe : BaseQuirk
{
    [SerializeField]
    float HealPercentages = .5f;
    public override void SingleActivate(NetworkIdentity user)
    {
        base.SingleActivate(user);
        CmdHealAndDestroy(user);
    }

    [Command]
    private void CmdHealAndDestroy(NetworkIdentity user)
    {
        HitPoints hitPoints = user.GetComponent<HitPoints>();
        if (hitPoints != null)
        {
            hitPoints.HealPercentageHealth(HealPercentages);
        }
        RequestDestroy();
    }

    [Server]
    private void RequestDestroy()
    {
        NetworkServer.Destroy(gameObject);   
    }
}
