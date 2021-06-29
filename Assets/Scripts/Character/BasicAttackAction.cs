using UnityEngine;
using Mirror;
using MD.UI;

namespace MD.Character
{
    public class BasicAttackAction : NetworkBehaviour
    {
        [SerializeField]
        protected int power = 2;

        [SerializeField]
        protected float cooldown = 1.6f;

        [SerializeField]
        protected int hitScore = 50;

        [SerializeField]
        protected float criticalMultiplier = 1.5f;

        [SerializeField]
        protected float immobilizeTime = .5f;

        [SerializeField]
        protected EnemyInAttackRangeDetect enemyDetect = null;

        [SerializeField]
        protected WeaponDamageZone damageZone = null;

        [SerializeField]
        protected PickaxeAnimatorController pickaxeAnimatorController = null;

        private Utils.Misc.Stopwatch cooldownStopwatch;

        public override void OnStartServer()
        {
            damageZone.OnDamagableCollide += GiveDamage;
            damageZone.OnCounterSuccessfully += OnCounterSuccessfully;
            damageZone.OnGetCountered += OnGetCountered;
        }

        public override void OnStopServer()
        {
            damageZone.OnDamagableCollide -= GiveDamage;
            damageZone.OnCounterSuccessfully -= OnCounterSuccessfully;
            damageZone.OnGetCountered -= OnGetCountered;
        }

        public override void OnStartAuthority()
        {
            cooldownStopwatch = gameObject.AddComponent<Utils.Misc.Stopwatch>();
            cooldownStopwatch.OnStop += () => EventSystems.EventManager.Instance.TriggerEvent(new AttackCooldownData(true));
            EventSystems.EventConsumer.GetOrAttach(gameObject).StartListening<AttackInvokeData>(HandleAttackInvoke);
        }

        [Client]
        protected void HandleAttackInvoke()
        {
            if (cooldownStopwatch.IsActive)
            {
                return;
            }

            cooldownStopwatch.Begin(cooldown);
            EventSystems.EventManager.Instance.TriggerEvent(new AttackCooldownData(false));
            enemyDetect.RaiseAttackDirEvent();
            pickaxeAnimatorController.Play();
            CmdAttemptSwingWeapon();
        }

        [Command]
        protected void CmdAttemptSwingWeapon() => damageZone.AttemptSwing();

        [Server]
        protected void GiveDamage(IDamagable damagable, bool isCritical)
        {
            var dmg = Mathf.RoundToInt(power * (isCritical ? criticalMultiplier : 1f));
            damagable.TakeDamage(netIdentity, dmg, isCritical);
            IncreaseScore(isCritical);
        }

        protected virtual void IncreaseScore(bool isCritical)
        {
            int score = Mathf.RoundToInt(hitScore * (isCritical ? criticalMultiplier : 1f));
            EventSystems.EventManager.Instance.TriggerEvent(new HitScoreObtainData(score));
        }

        protected virtual void OnCounterSuccessfully(Vector2 counterVect) => TargetOnCounterSuccessfully(counterVect);

        protected virtual void OnGetCountered(Vector2 counterVect) => TargetOnGetCountered(counterVect);

        [TargetRpc]
        protected void TargetOnCounterSuccessfully(Vector2 counterVect) => EventSystems.EventManager.Instance.TriggerEvent(new CounterSuccessData(counterVect));

        [TargetRpc]
        protected void TargetOnGetCountered(Vector2 counterVect)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new GetCounteredData(counterVect, immobilizeTime));
        }
    }
}
