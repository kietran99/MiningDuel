using UnityEngine;

namespace MD.AI
{
    public class PB_ThrowProjectileAway : FSMState
    {
        public PB_ThrowProjectileAway(PlayerBot bot) : base(bot)
        {
            name = STATE.THROW_PROJECTILE_AWAY;
        }

        public override void Enter()
        {
            Debug.Log("Throwing Projectile Away");
            if (!bot.Throwable)
            {
                nextState = new PB_Idle(bot);
                stage = EVENT.EXIT;
                return;
            }
            bot.ThrowProjectile();
            base.Enter();
        }
        public override void Update()
        {
            base.Update();
            //if hsnt throw bomb yet
            if (bot.Throwable) return;
            
            stage = EVENT.EXIT;
            nextState = new PB_Idle(bot);
        }
    }
}

