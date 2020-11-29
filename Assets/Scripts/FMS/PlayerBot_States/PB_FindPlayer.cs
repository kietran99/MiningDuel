using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PB_FindPlayer : FMSState
{
    private int currentIndex = 0;
    private bool lastSeenPlayerArrived = false;
    private float elapsedTime = 0f;
    public PB_FindPlayer(PlayerBot bot) : base(bot)
    {
        name = STATE.FINDPLAYER;
    }

    public override void Enter()
    {
        Debug.Log("finding player");
        base.Enter();
        bot.movePos = bot.lastSeenPlayer;
        bot.isMoving = true;
    }
    public override void Update()
    {
        base.Update();
        if (bot.CanSeePlayer())
        {
            stage = EVENT.EXIT;
            if (bot.CanThrow())
            {
                Debug.Log("found Player and has bomb");
                nextState = new PB_ThrowBomb(bot);
            }
            else
            {
                Debug.Log("found Player and doesnt have bomb");
                nextState = new PB_FindDiggable(bot, true);
            }
            return;
        }
        if (bot.CanThrow() && elapsedTime >= bot.holdBombTime)
        {
            Debug.Log("has bomb and out of time");
            stage = EVENT.EXIT;
            nextState = new PB_ThrowBombAway(bot);
        }
        if (bot.isMoving == false)
        {
            if (!lastSeenPlayerArrived)
            {
                lastSeenPlayerArrived = true;
                currentIndex =  bot.GetClosestWayPointIndex();
            }
            else{
                 currentIndex++;
                if (currentIndex > bot.checkPoints.Count -1) currentIndex = 0;
            }
            bot.movePos = bot.checkPoints[currentIndex].transform.position;
            bot.isMoving = true;
        }
        elapsedTime += Time.deltaTime;
    }
}
