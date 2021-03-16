using UnityEngine;

namespace MD.AI
{
    public class PB_Dig : FSMState
    {
        private bool forBomb = false;
        public PB_Dig(PlayerBot bot, bool forBomb) : base(bot)
        {
            name = STATE.DIG;
            this.forBomb  = forBomb;
        }

        public override void Enter()
        {
            // Debug.Log("Digging");
            base.Enter();
            if (!bot.CanDig(forBomb))
            {
                stage = EVENT.EXIT;
                nextState = new PB_Idle(bot);
                return;
            }
        }
        public override void Update()
        {
            base.Update();

            if (bot.isDigging) return;

            if (!bot.CanDig(forBomb))
            {
                stage = EVENT.EXIT;
                if (bot.Throwable)
                {
                    if (bot.CanSeePlayer)
                    {
                        // Debug.Log("Dig complete - Obtained Projectile - Can see Player");
                        nextState = new PB_ThrowProjectile(bot);
                    }
                    else
                    {
                        // Debug.Log("Dig complete - Obtained Projectile - Cannot see Player");
                        nextState = new PB_FindPlayer(bot);
                    }

                    return;
                }

                // Debug.Log("Dig complete - No Projectile");
                nextState = new PB_Idle(bot);
                return;
            }
            
            bot.Dig();
        }
    }
}