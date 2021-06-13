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
            EventSystems.EventConsumer.GetOrAttach(gameObject).StartListening<AttackInvokeData>(HandleAttackInvoke);
        }

        [Client]
        protected void HandleAttackInvoke(AttackInvokeData _)
        {
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
            EventSystems.EventManager.Instance.TriggerEvent(new HitScoreObtainData(Mathf.RoundToInt(hitScore * (isCritical ? criticalMultiplier : 1f))));
        }

        protected void OnCounterSuccessfully(Vector2 counterVect) => TargetOnCounterSuccessfully(counterVect);

        protected void OnGetCountered(Vector2 counterVect) => TargetOnGetCountered(counterVect);

        [TargetRpc]
        protected void TargetOnCounterSuccessfully(Vector2 counterVect) => EventSystems.EventManager.Instance.TriggerEvent(new CounterSuccessData(counterVect));

        [TargetRpc]
        protected void TargetOnGetCountered(Vector2 counterVect)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new GetCounteredData(counterVect, immobilizeTime));
        }
    }
}
