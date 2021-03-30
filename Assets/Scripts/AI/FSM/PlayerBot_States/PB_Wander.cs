using UnityEngine;

namespace MD.AI
{
    public class PB_Wander : FSMState
{
    private int currentIndex = 0;
    private bool forBomb = false;

    public PB_Wander(PlayerBot bot, bool forBomb) : base(bot)
    {
        name = STATE.WANDER;
        this.forBomb  = forBomb;
    }

    public override void Enter()
    {
        // Debug.Log("Wandering");
        base.Enter();
        currentIndex =  bot.GetClosestWayPointIndex();
        // if(bot.SetMovePosition(bot.checkPoints[currentIndex].transform.position))
        //     bot.StartMoving();
        bot.StartWandering();
    }
    public override void Update()
    {
        base.Update();
        if (!bot.IsMoving)
        {
            if (bot.GetClosestDiggable(out _,forBomb))
            {
                // Debug.Log("Found a " + (forBomb ? "Projectile" : "Gem"));
                stage = EVENT.EXIT;
                nextState = new PB_FindDiggable(bot, forBomb);
            }
            else
            {
                bot.StartWandering();
            }
        }
    }
}
}