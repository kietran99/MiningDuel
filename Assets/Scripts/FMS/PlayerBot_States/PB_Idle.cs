using UnityEngine;
public class PB_Idle : FMSState
{
    public PB_Idle(PlayerBot bot) : base(bot)
    {
        name = STATE.IDLE;
    }

    public override void Enter()
    {
        Debug.Log("idle");
        base.Enter();
    }
    public override void Update()
    {
        base.Update();
        stage = EVENT.EXIT;
        if (bot.GetCurrentScore() < bot.player.GetCurrentScore() && bot.GetCurrentScore() >= 20)
        {
            Debug.Log("<player score and >= 20 score");
            nextState = new PB_FindPlayer(bot);
        }
        else
        {
            Debug.Log(">= player score or <20 score");
            nextState = new PB_FindDiggable(bot, false);
        }
    }
}

