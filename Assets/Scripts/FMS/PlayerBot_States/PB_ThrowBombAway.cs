using UnityEngine;
public class PB_ThrowBombAway : FMSState
{
    public PB_ThrowBombAway(PlayerBot bot) : base(bot)
    {
        name = STATE.THROWBOMBAWAY;
    }

    public override void Enter()
    {
        Debug.Log("throw bomb away");
        if (!bot.CanThrow())
        {
            nextState = new PB_Idle(bot);
            stage = EVENT.EXIT;
            return;
        }
        bot.ThrowBomb();
        base.Enter();
    }
    public override void Update()
    {
        base.Update();
        //if hsnt throw bomb yet
        if (bot.CanThrow()) return;
        stage = EVENT.EXIT;
        nextState = new PB_Idle(bot);
    }
}

