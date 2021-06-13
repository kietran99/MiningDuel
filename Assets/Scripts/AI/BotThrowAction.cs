using UnityEngine;
using MD.Character;
using MD.Diggable.Projectile;
using System.Collections;

namespace MD.AI
{
    public class BotThrowAction : ThrowAction
    {
        private float WaitTime = .5f;
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
            StartCoroutine(WaitAndThrowProjectile());
        }

        public IEnumerator WaitAndThrowProjectile()
        {
            yield return new WaitForSeconds(WaitTime);
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
