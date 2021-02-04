using UnityEngine;
using MD.Character;
using MD.Diggable.Projectile;

namespace MD.AI
{
    public class BotThrowAction : ThrowAction
    {
        private BotAnimator botAnimator;

        private void Start()
        {
            botAnimator = GetComponent<BotAnimator>();
        }

        public override void SetHoldingProjectile(ProjectileLauncher proj)
        {
            Debug.Log("Bot Set Holding Projectile");
            holdingProjectile = proj;
            botAnimator.SetHoldState();
        }

        public void ThrowProjectile()
        {
            Vector2 dir = Vector2.one;

            if (ServiceLocator.Resolve<Player>(out Player player))
            {
                dir = (player.transform.position - transform.position).normalized;
            }

            botAnimator.RevertToIdleState();
            base.CmdThrow(dir.x, dir.y);
            holdingProjectile = null;
        }

        public bool IsHoldingProjectile => holdingProjectile != null;
    }
}
