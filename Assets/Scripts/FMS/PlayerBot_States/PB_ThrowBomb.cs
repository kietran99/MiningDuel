using UnityEngine;

namespace MD.AI
{
    public class PB_ThrowBomb : FSMState
    {
        public PB_ThrowBomb(PlayerBot bot) : base(bot)
        {
            name = STATE.THROWBOMB;
        }

        public override void Enter()
        {
            Debug.Log("throw bomb");
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
}

