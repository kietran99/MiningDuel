using UnityEngine;

namespace MD.AI
{
    public class PB_AttackPlayer : FSMState
    {
        private float attackCoolDown = 1f; 

        private float nextCanAttackTime = 0f;
        public PB_AttackPlayer(PlayerBot bot) : base(bot)
        {
            name = STATE.ATTACKPLAYER;
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {    
            base.Update(); 
            stage = EVENT.EXIT;
            nextState = new PB_ChasePlayer(bot);
            if (Time.time > nextCanAttackTime)
            {
                //attack
                Debug.Log("attack");
                bot.Attack();
                nextCanAttackTime = Time.time + attackCoolDown;  
            }
        }
    }
}

