
public class PB_ThrowBomb : FMSState
{
    public PB_ThrowBomb(PlayerBot bot) : base(bot)
    {
        name = STATE.THROWBOMB;
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

