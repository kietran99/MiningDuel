
using UnityEngine;
using MD.Quirk;
using Mirror;
using MD.Character;
public class AvatarPotion : BaseQuirk
{
    [SerializeField]
    int addDigPower = 2;

    [SerializeField]
    float speedPercentage = .3f;

    [SerializeField]
    float time = 10f;

    public override void SingleActivate(NetworkIdentity user)
    {
        Debug.Log("active speed potion");
        base.SingleActivate(user);
        MoveAction moveAction = user.GetComponent<MoveAction>();
        if (moveAction != null)
        {
            moveAction.CmdModifySpeed(speedPercentage,time);
        }
        DigAction digAction = user.GetComponent<DigAction>();
        if (digAction != null)
        {
            digAction.IncreaseDigPower(addDigPower,time);
        }
        CmdRequestDestroy();
    }

    [Command]
    private void CmdRequestDestroy()
    {
        NetworkServer.Destroy(gameObject);   
    }
}
