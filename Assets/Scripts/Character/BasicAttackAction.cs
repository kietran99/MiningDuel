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

        public override void OnStartAuthority()
        {
            EventSystems.EventManager.Instance.StartListening<AttackInvokeData>(HandleAttackInvoke);
            enemyDetect.OnTrackingTargetsChanged += ToggleMainAction;
            damageZone.OnDamagableCollide += CmdNewAttack;
            damageZone.OnGetCountered += HandleGetCountered;
        }

        public override void OnStopClient()
        {
            EventSystems.EventManager.Instance.StopListening<AttackInvokeData>(HandleAttackInvoke);
            enemyDetect.OnTrackingTargetsChanged -= ToggleMainAction;
            damageZone.OnDamagableCollide -= CmdNewAttack;
            damageZone.OnGetCountered -= HandleGetCountered;
        }

        [Client]
        private void HandleAttackInvoke(AttackInvokeData _)
        {
            pickaxeAnimatorController.Play();
            damageZone.AttemptSwing();
        }

        private void ToggleMainAction(bool targetsInRange)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new Character.MainActionToggleData(targetsInRange ? MainActionType.ATTACK : MainActionType.DIG));
        }

        [Command]
        private void CmdNewAttack(NetworkIdentity damagable)
        {
            damagable.GetComponent<IDamagable>().TakeDamage(netIdentity, power);
        }

        private void HandleGetCountered(Vector2 counterVect)
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
                // GetCounter(new Vector2(-2f, -2f), immobilizeTime);
                EventSystems.EventManager.Instance.TriggerEvent(new AttackInvokeData());
            }
        }
    #endif
    }
}
