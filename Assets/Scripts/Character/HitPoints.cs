using UnityEngine;
using Mirror;

namespace MD.Character
{
    public class HitPoints : NetworkBehaviour, IDamagable
    {
        [SerializeField]
        private int maxHP = 100;

        [SerializeField]
        private int minHP = 0;

        [SerializeField]
        [SyncVar(hook = nameof(OnCurrentHPSync))]
        private int currentHP = 100;

        [SerializeField]
        private VisualEffects.DamagedVFX damagedVFX = null;

        public System.Action<uint> OnOutOfHP { get; set; }

        [Server]
        public void TakeDamage(NetworkIdentity source, int dmg)
        {
            currentHP = Mathf.Clamp(currentHP - dmg, minHP, maxHP);
            TargetOnDamageGiven(source.connectionToClient, dmg);
            TargetOnDamageTaken((transform.position - source.transform.position).normalized);

            if (!currentHP.Equals(minHP))
            {
                return;
            }

            OnOutOfHP?.Invoke(netId);
        }

        [TargetRpc]
        private void TargetOnDamageGiven(NetworkConnection atker, int dmg)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new DamageGivenData(transform.position, dmg));
        }

        [TargetRpc]
        private void TargetOnDamageTaken(Vector2 atkDir) => EventSystems.EventManager.Instance.TriggerEvent(new DamageTakenData(netId, atkDir));

        private void OnCurrentHPSync(int oldCurHP, int newCurHP)
        {
            damagedVFX.Play();

            if (hasAuthority)
            {
                OnAuthorityCurrentHPSync(oldCurHP, newCurHP);
            }
        }

        protected virtual void OnAuthorityCurrentHPSync(int oldCurHP, int newCurHP)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new HPChangeData(oldCurHP, newCurHP, maxHP));
        }

    #region TEST_TAKE_DMG
    #if UNITY_EDITOR
        [ClientCallback]
        void Update()
        {
            if (!hasAuthority)
            {
                return;
            }

            if (GetComponent<AI.PlayerBot>() != null)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                CmdTakeDamage();
            }
        }

        [Command]
        void CmdTakeDamage() => TakeDamage(this.netIdentity, 20);
    #endif
    #endregion
    }
}