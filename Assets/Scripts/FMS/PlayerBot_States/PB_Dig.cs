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
        Debug.Log("dig");
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
                   Debug.Log("dig complete, has bomb and can see player");
                   nextState = new PB_ThrowBomb(bot);
               }
               else
               {
                   Debug.Log("dig complete, has bomb and cant see player");
                   nextState = new PB_FindPlayer(bot);
               }
               return;
            }
            Debug.Log("dig complete, doesnt have bomb");
            nextState = new PB_Idle(bot);
            return;
        }
        bot.Dig();
    }

}
