using UnityEngine;

namespace MD.AI
{
    public class PB_ChasePlayer : FSMState
    {
        private float squareAttackRange = 2.25f;
        private float distanceCheckInterval = .1f;
        private float planCheckInterval = 1f;
        private float chaseTime = 5f;

        private float stopChasingTime = 0f;
        private float nextDistanceCheck = 0f;
        private float nextPlanCheck = 0f;


        public PB_ChasePlayer(PlayerBot bot) : base(bot)
        {
            name = STATE.CHASEPLAYER;
        }

        public override void Enter()
        {
            base.Enter();
            nextPlanCheck = 0f;
            nextDistanceCheck = 0f;
            stopChasingTime = Time.time + chaseTime;
        }

        public override void Update()
        {               
            base.Update();
            if (Time.time >= stopChasingTime || !bot.CanSeePlayer)
            {
                stage = EVENT.EXIT;
                nextState = new PB_Idle(bot);
                return;
            }

            if (Time.time >= nextDistanceCheck)
            {
                nextDistanceCheck = Time.time + distanceCheckInterval;
                if (bot.GetSquarePlayerDistance() <= squareAttackRange)
                {
                    bot.StopMoving();
                    stage = EVENT.EXIT;
                    nextState = new PB_AttackPlayer(bot);
                    return;
                }
            }

            if (!bot.IsMoving || Time.time >= nextPlanCheck)
            {
                nextPlanCheck = Time.time + planCheckInterval;
                bot.SetMovePosition(bot.Target.transform.position);
                bot.StartMoving();
            }
        }
    }
}

