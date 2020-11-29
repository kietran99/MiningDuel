using UnityEngine;
public class PB_Idle : FMSState
{
    public PB_Idle(PlayerBot bot) : base(bot)
    {
        name = STATE.IDLE;
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        base.Update();
        stage = EVENT.EXIT;
        if (bot.GetCurrentScore() < bot.player.GetCurrentScore() && bot.GetCurrentScore() >= 20)
        {
            nextState = new PB_FindPlayer(bot);
        }
        else
        {
            nextState = new PB_FindDiggable(bot, false);
        }
    }
}

