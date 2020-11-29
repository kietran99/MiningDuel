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
        base.Enter();
        foundLocation =  bot.GetClosestDiggable(out bot.movePos, forBomb);
        bot.isMoving = true;
    }
    public override void Update()
    {
        base.Update();
        if (!foundLocation)
        {
            stage = EVENT.EXIT;
            nextState = new PB_Wander(bot, forBomb);
            return;
        }
        if (bot.isMoving == false)
        {
            stage = EVENT.EXIT;
            nextState = new PB_Dig(bot, forBomb);
        }
    }
}
