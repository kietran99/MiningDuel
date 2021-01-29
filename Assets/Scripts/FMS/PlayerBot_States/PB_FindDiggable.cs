using UnityEngine;

namespace MD.AI
{
    public class PB_FindDiggable : FSMState
    {
        private bool foundLocation = false;
        private bool forBomb = false;
        public PB_FindDiggable(PlayerBot bot, bool forBomb) : base(bot)
        {
            name = STATE.FINDDIGGABLE;
            this.forBomb = forBomb;
        }

        public override void Enter()
        {
            Debug.Log("find " + (forBomb?"bomb":"gem"));
            base.Enter();
            Vector2 movePos = Vector2.zero;
            foundLocation =  bot.GetClosestDiggable(out movePos, forBomb);
            if(foundLocation)
            {
                bot.SetMovePosition(movePos);
                bot.StartMoving();
            }
        }
        public override void Update()
        {
            base.Update();
            if (!foundLocation)
            {
                Debug.Log("no " + (forBomb?"bomb":"gem") + " in sonar range");
                stage = EVENT.EXIT;
                nextState = new PB_Wander(bot, forBomb);
                return;
            }
            if (!bot.IsMoving())
            {
                Debug.Log("found " + (forBomb?"bomb":"gem") + " in sonar range");
                stage = EVENT.EXIT;
                nextState = new PB_Dig(bot, forBomb);
            }
        }
    }
}
