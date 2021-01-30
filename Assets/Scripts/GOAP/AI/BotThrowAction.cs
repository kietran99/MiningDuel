using UnityEngine;
using MD.Character;
using MD.Diggable.Projectile;

namespace MD.AI
{
    public class BotThrowAction : ThrowAction
    {
        public override void SetHoldingProjectile(ProjectileLauncher proj)
        {
            Debug.Log("set holding projectile on bot");
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
            base.CmdThrowProjectile(dir.x, dir.y, basePower);
            holdingProjectile = null;
        }

        public bool IsHoldingProjectile => holdingProjectile != null;
    }
}
