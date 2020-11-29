using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PB_Wander : FMSState
{
    private int currentIndex = 0;
    private bool forBomb = false;

    public PB_Wander(PlayerBot bot, bool forBomb) : base(bot)
    {
        name = STATE.WANDER;
        this.forBomb  = forBomb;
    }

    public override void Enter()
    {
        base.Enter();
        currentIndex =  bot.GetClosestWayPointIndex();
        bot.movePos = bot.checkPoints[currentIndex].transform.position;
        bot.isMoving = true;
    }
    public override void Update()
    {
        base.Update();
        if (bot.isMoving == false)
        {
            if (bot.GetClosestDiggable(out _,forBomb))
            {
                stage = EVENT.EXIT;
                nextState = new PB_Dig(bot, forBomb);
            }
            else
            {
                currentIndex++;
                if (currentIndex > bot.checkPoints.Count -1) currentIndex = 0;
                bot.movePos = bot.checkPoints[currentIndex].transform.position;
                bot.isMoving = true;
            }
        }
    }
}
