using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PB_Dig : FMSState
{
    private bool forBomb = false;
    public PB_Dig(PlayerBot bot, bool forBomb) : base(bot)
    {
        name = STATE.DIG;
        this.forBomb  = forBomb;
    }

    public override void Enter()
    {
        base.Enter();
        if (!bot.CanDig(forBomb))
        {
            stage = EVENT.EXIT;
            nextState = new PB_Idle(bot);
            return;
        }
    }
    public override void Update()
    {
        base.Update();
        if (bot.isDigging) return;
        if (!bot.CanDig(forBomb))
        {
            stage = EVENT.EXIT;
            if (bot.CanThrow())
            {
               if (bot.CanSeePlayer())
               {
                   nextState = new PB_ThrowBomb(bot);
               }
               else
               {
                   nextState = new PB_FindPlayer(bot);
               }
            }
            nextState = new PB_Idle(bot);
            return;
        }
        bot.Dig();
    }

}
