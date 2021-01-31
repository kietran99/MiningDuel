using UnityEngine;

namespace MD.AI
{
    public class PB_FindDiggable : FSMState
    {
        private bool foundLocation = false;
        private bool forBomb = false;
        public PB_FindDiggable(PlayerBot bot, bool forBomb) : base(bot)
        {
            name = STATE.FINDING_DIGGABLE;
            this.forBomb = forBomb;
        }

        public override void Enter()
        {
            Debug.Log("Finding: " + (forBomb? "Projectile" : "Gem"));
            base.Enter();
            Vector2 movePos = Vector2.zero;
            foundLocation =  bot.GetClosestDiggable(out movePos, forBomb);
            if(foundLocation)
            {
                if (!bot.SetMovePosition(movePos))
                {
                    foundLocation = false;
                    return;
                }
                bot.StartMoving();
            }
        }
        public override void Update()
        {
            base.Update();
            if (!foundLocation)
            {
                Debug.Log("No " + (forBomb? "Projectile" : "Gem") + " in Scan Range");
                stage = EVENT.EXIT;
                nextState = new PB_Wander(bot, forBomb);
                return;
            }
            if (!bot.IsMoving)
            {
                Debug.Log("Found " + (forBomb? "Projectile" : "Gem") + " in Scan range");
                stage = EVENT.EXIT;
                nextState = new PB_Dig(bot, forBomb);
            }
        }
    }
}
