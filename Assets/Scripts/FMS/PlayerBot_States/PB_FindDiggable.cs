using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PB_FindDiggable : FMSState
{
    private bool foundLocation = false;
    private bool forBomb = false;
    public PB_FindDiggable(PlayerBot bot, bool forBomb) : base(bot)
    {
        name = STATE.FINDDIGGABLE;
        this.forBomb = forBomb;
    }

    public override void Enter()
    {
        Debug.Log("find " + (forBomb?"bomb":"gem"));
        base.Enter();
        foundLocation =  bot.GetClosestDiggable(out bot.movePos, forBomb);
        bot.isMoving = true;
    }
    public override void Update()
    {
        base.Update();
        if (!foundLocation)
        {
            Debug.Log("no " + (forBomb?"bomb":"gem") + " in sonar range");
            stage = EVENT.EXIT;
            nextState = new PB_Wander(bot, forBomb);
            return;
        }
        if (bot.isMoving == false)
        {
            Debug.Log("found " + (forBomb?"bomb":"gem") + " in sonar range");
            stage = EVENT.EXIT;
            nextState = new PB_Dig(bot, forBomb);
        }
    }
}
