
public class PB_ThrowBombAway : FMSState
{
    public PB_ThrowBombAway(PlayerBot bot) : base(bot)
    {
        name = STATE.THROWBOMBAWAY;
    }

    public override void Enter()
    {
        bot.ThrowBomb();
        base.Enter();
    }
    public override void Update()
    {
        base.Update();
        stage = EVENT.EXIT;
        nextState = new PB_Idle(bot);
    }
}

