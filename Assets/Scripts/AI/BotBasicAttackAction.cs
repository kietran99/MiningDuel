using UnityEngine;

namespace MD.AI
{
    public class BotBasicAttackAction : MD.Character.BasicAttackAction
    {
        [SerializeField]
        private BotAnimator animatorController = null;

        [SerializeField]
        private PlayerBot bot = null;

        public override void OnStartAuthority()
        {}

        public void Attack()
        {
            animatorController.PlayBasicAttack(enemyDetect.CurAtkDir);
            pickaxeAnimatorController.Play();
            CmdAttemptSwingWeapon();
        }

        protected override void IncreaseScore(bool isCritical)
        {
            bot.IncreaseScore(Mathf.RoundToInt(hitScore * (isCritical ? criticalMultiplier : 1f)));
        }
    }
}