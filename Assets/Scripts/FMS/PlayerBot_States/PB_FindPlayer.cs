using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PB_FindPlayer : FMSState
{
    private bool forBomb = false;
    private int currentIndex = 0;

    private float elapsedTime = 0f;

    public PB_FindPlayer(PlayerBot bot) : base(bot)
    {
        name = STATE.FINDPLAYER;
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
        if (bot.CanSeePlayer())
        {
            stage = EVENT.EXIT;
            nextState = new PB_ThrowBomb(bot);
            return;
        }
        if (elapsedTime >= bot.holdBombTime)
        {
            stage = EVENT.EXIT;
            nextState = new PB_ThrowBombAway(bot);
        }
        if (bot.isMoving == false)
        {
            currentIndex++;
            if (currentIndex > bot.checkPoints.Count -1) currentIndex = 0;
            bot.movePos = bot.checkPoints[currentIndex].transform.position;
            bot.isMoving = true;
        }
        elapsedTime += Time.deltaTime;
    }
}
