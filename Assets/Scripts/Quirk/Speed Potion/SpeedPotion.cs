using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD.Quirk;
using Mirror;
using MD.Character;
public class SpeedPotion : BaseQuirk
{
    [SerializeField]
    float speedPercentage = 0f;
    [SerializeField]
    float time = 3f;

    public override void SingleActivate(NetworkIdentity user)
    {
        Debug.Log("active speed potion");
        base.SingleActivate(user);
        MoveAction moveAction = user.GetComponent<MoveAction>();
        if (moveAction != null)
        {
            moveAction.CmdModifySpeed(speedPercentage,time);
        }

        CmdRequestDestroy();
    }

    [Command]
    private void CmdRequestDestroy()
    {
        NetworkServer.Destroy(gameObject);   
    }

}
