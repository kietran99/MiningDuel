using UnityEngine;
using Mirror;
using MD.UI;
using System.Collections.Generic;

namespace MD.Character
{
    public class BasicAttackAction : NetworkBehaviour
    {
        [SerializeField]
        private int power = 2;

        [SerializeField]
        private PickaxeAnimatorController pickaxeController = null;

        private List<NetworkIdentity> damagableList = new List<NetworkIdentity>(4);

        public override void OnStartAuthority()
        {
            EventSystems.EventManager.Instance.StartListening<AttackInvokeData>(HandleAttackInvoke);
            pickaxeController.OnDamagableEnter += AddToDamagableList;
            pickaxeController.OnDamagableExit += RemoveFromDamagableList;
        }

        public override void OnStopClient()
        {
            EventSystems.EventManager.Instance.StopListening<AttackInvokeData>(HandleAttackInvoke);
            pickaxeController.OnDamagableEnter -= AddToDamagableList;
            pickaxeController.OnDamagableExit -= RemoveFromDamagableList;
        }

        [Client]
        private void HandleAttackInvoke(AttackInvokeData _)
        {
            if (damagableList.Count.Equals(0))
            {
                return;
            }

            pickaxeController.Play(damagableList.Random().transform.position - transform.position);
            CmdAttack(damagableList);
        }
          
        [Command]
        private void CmdAttack(List<NetworkIdentity> damagableList) => damagableList.ForEach(damagable => damagable.GetComponent<IDamagable>().TakeDamage(power));

        private void AddToDamagableList(NetworkIdentity damagable)
        {
            damagableList.Add(damagable);
            // Debug.Log("Damagable List Size: " + damagableList.Count);
            if (damagableList.Count.Equals(1)) 
            {
                EventSystems.EventManager.Instance.TriggerEvent(new Character.MainActionToggleData(MainActionType.ATTACK));
            }
        }

        private void RemoveFromDamagableList(NetworkIdentity damagable)
        {
            damagableList.Remove(damagable);
            // Debug.Log("Damagable List Size: " + damagableList.Count);
            if (damagableList.Count.Equals(0)) 
            {
                EventSystems.EventManager.Instance.TriggerEvent(new Character.MainActionToggleData(MainActionType.DIG));
            }
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
