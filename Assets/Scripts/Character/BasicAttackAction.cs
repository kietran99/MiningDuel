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
            EventSystems.EventManager.Instance.StartListening<AttackInvokeData>(HandleAttackInvoke);
            enemyDetect.OnTrackingTargetsChanged += ToggleMainAction;
        }

        public override void OnStopClient()
        {
            EventSystems.EventManager.Instance.StopListening<AttackInvokeData>(HandleAttackInvoke);
            enemyDetect.OnTrackingTargetsChanged -= ToggleMainAction;
        }

        [Client]
        private void HandleAttackInvoke(AttackInvokeData _)
        {
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
        private void GiveDamage(NetworkIdentity damagable)
        {
            damagable.GetComponent<IDamagable>().TakeDamage(netIdentity, power);
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

    #if UNITY_EDITOR
        [ClientCallback]
        void Update()
        {
            if (!hasAuthority)
            {
                return;
            }    

            if (Input.GetMouseButtonDown(1))
            {
                EventSystems.EventManager.Instance.TriggerEvent(new AttackInvokeData());
            }
        }
    #endif
    }
}
