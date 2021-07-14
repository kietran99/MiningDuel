using UnityEngine;

namespace MD.AI
{
    public class PB_ThrowProjectile : FSMState
    {

        public PB_ThrowProjectile(PlayerBot bot) : base(bot)
        {
            name = STATE.THROW_PROJECTILE;
        }

        public override void Enter()
        {
            Debug.Log("Throwing Projectile");
            if (!bot.Throwable)
            {
                nextState = new PB_Idle(bot);
                stage = EVENT.EXIT;
                return;
            }
            bot.StopMoving();
            bot.ThrowProjectile();
            base.Enter();
        }
        public override void Update()
        {
            base.Update();

            if (bot.IsHoldingProjectile()) return;
            stage = EVENT.EXIT;
            nextState = new PB_Idle(bot);
        }
    }
}

