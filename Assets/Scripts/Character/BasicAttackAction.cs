using UnityEngine;
using Mirror;
using MD.UI;

namespace MD.Character
{
    public class BasicAttackAction : NetworkBehaviour
    {
        [SerializeField]
        private int power = 2;

        [SerializeField]
        private int hitScore = 50;

        [SerializeField]
        private float criticalMultiplier = 1.5f;

        [SerializeField]
        private float immobilizeTime = .5f;

        [SerializeField]
        private EnemyInAttackRangeDetect enemyDetect = null;

        [SerializeField]
        private WeaponDamageZone damageZone = null;

        [SerializeField]
        private PickaxeAnimatorController pickaxeAnimatorController = null;

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
            var eventConsumer = EventSystems.EventConsumer.GetOrAttach(gameObject);
            eventConsumer.StartListening<AttackInvokeData>(HandleAttackInvoke);
        }

        [Client]
        private void HandleAttackInvoke(AttackInvokeData _)
        {
            enemyDetect.RaiseAttackDirEvent();
            pickaxeAnimatorController.Play();
            CmdAttemptSwingWeapon();
        }

        [Command]
        private void CmdAttemptSwingWeapon() => damageZone.AttemptSwing();

        private void ToggleMainAction(bool targetsInRange)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new Character.MainActionToggleData(targetsInRange ? MainActionType.ATTACK : MainActionType.DIG));
        }

        [Server]
        private void GiveDamage(IDamagable damagable, bool isCritical)
        {
            var dmg = Mathf.RoundToInt(power * (isCritical ? criticalMultiplier : 1f));
            damagable.TakeDamage(netIdentity, dmg, isCritical);
            EventSystems.EventManager.Instance.TriggerEvent(new HitScoreObtainData(Mathf.RoundToInt(hitScore * (isCritical ? criticalMultiplier : 1f))));
        }

        private void OnCounterSuccessfully(Vector2 counterVect) => TargetOnCounterSuccessfully(counterVect);

        private void OnGetCountered(Vector2 counterVect) => TargetOnGetCountered(counterVect);

        [TargetRpc]
        private void TargetOnCounterSuccessfully(Vector2 counterVect) => EventSystems.EventManager.Instance.TriggerEvent(new CounterSuccessData(counterVect));

        [TargetRpc]
        private void TargetOnGetCountered(Vector2 counterVect)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new GetCounteredData(counterVect, immobilizeTime));
        }
    }
}
