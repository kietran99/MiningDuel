using UnityEngine;
using MD.Character;
using MD.Diggable.Projectile;

namespace MD.AI
{
    public class BotThrowAction : ThrowAction
    {
        public override void SetHoldingProjectile(ProjectileLauncher proj)
        {
            Debug.Log("Bot Set Holding Projectile");
            base.SetHoldingProjectile(proj);
            BotAnimator animator = GetComponent<BotAnimator>();

            if (animator != null) animator.SetHoldState();
        }

        public void ThrowProjectile()
        {
            //find player pos;
            Player player;
            Vector2 dir = Vector2.one;

            if (ServiceLocator.Resolve<Player>(out player))
            {
                dir = (player.transform.position - transform.position).normalized;
            }

            BotAnimator animator = GetComponent<BotAnimator>();
            if (animator != null) animator.RevertToIdleState();
            base.CmdThrow(dir.x, dir.y);
            holdingProjectile = null;
        }

        public bool IsHoldingProjectile => holdingProjectile != null;
    }
}
