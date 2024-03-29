﻿using UnityEngine;

namespace MD.AI
{
    public class BotBasicAttackAction : MD.Character.BasicAttackAction
    {
        [SerializeField]
        private BotAnimator animatorController = null;

        [SerializeField]
        private PlayerBot bot = null;

        public float Cooldown => cooldown;

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

        protected override void OnCounterSuccessfully(Vector2 counterVect)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new BotCounterSuccessData(counterVect));
        }
        protected override void OnGetCountered(Vector2 counterVect)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new BotGetCounteredData(counterVect, immobilizeTime));
        }
    }
}