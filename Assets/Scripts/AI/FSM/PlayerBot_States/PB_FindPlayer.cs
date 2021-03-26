using UnityEngine;

namespace MD.AI
{
    public class PB_FindPlayer : FSMState
    {
        private int currentIndex = 0;
        private bool lastSeenPlayerArrived = false;
        private float elapsedTime = 0f;

        public PB_FindPlayer(PlayerBot bot) : base(bot)
        {
            name = STATE.FIND_PLAYER;
        }

        public override void Enter()
        {
            Debug.Log("Finding Player");
            base.Enter();
            if(bot.SetMovePosition(bot.lastSeenPlayer))
                bot.StartMoving();
        }

        public override void Update()
        {
            base.Update();
            if (bot.CanSeePlayer)
            {
                stage = EVENT.EXIT;
                if (bot.Throwable)
                {
                    Debug.Log("Found Player - Obtained Projectile");
                    nextState = new PB_ThrowProjectile(bot);
                }
                else
                {
                    Debug.Log("Found Player - No Projectile");
                    nextState = new PB_FindDiggable(bot, true);
                }
                return;
            }
            if (bot.Throwable && elapsedTime >= bot.holdBombTime)
            {
                Debug.Log("Holding Projectile - Out of Time");
                stage = EVENT.EXIT;
                nextState = new PB_ThrowProjectileAway(bot);
            }
            if (!bot.IsMoving)
            {
                if (!lastSeenPlayerArrived)
                {
                    lastSeenPlayerArrived = true;
                    currentIndex =  bot.GetClosestWayPointIndex();
                }
                else{
                    currentIndex++;
                    if (currentIndex > bot.checkPoints.Count -1) currentIndex = 0;
                }
                //hard coded middle of the map
                bot.SetMovePosition(new Vector2(18f,15f));
                bot.StartMoving();
            }
            elapsedTime += Time.deltaTime;
        }
    }
}