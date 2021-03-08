using UnityEngine;

namespace MD.AI
{
    public class PB_Idle : FSMState
    {
        public PB_Idle(PlayerBot bot) : base(bot)
        {
            name = STATE.IDLE;
        }

        public override void Enter()
        {
            Debug.Log("Idling");
            base.Enter();
        }
        public override void Update()
        {
            base.Update();
            stage = EVENT.EXIT;
            if (bot.CurrentScore < bot.Target.FinalScore && bot.CurrentScore >= 20)
            {
                Debug.Log("Bot Score: < Player Score & >= 20");
                nextState = new PB_FindPlayer(bot);
            }
            else
            {
                Debug.Log("Bot Score: >= Player Score or < 20");
                nextState = new PB_FindDiggable(bot, false);
            }
        }
    }
}

