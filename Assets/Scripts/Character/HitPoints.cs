using UnityEngine;
using Mirror;

namespace MD.Character
{
    public class HitPoints : NetworkBehaviour, IDamagable, AI.TheWarden.IWardenDamagable
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

        [SerializeField]
        private ParticleSystem HealEffect = null;

        [Server]
        public void TakeWardenDamage(int dmg)
        {
            currentHP = Mathf.Clamp(currentHP - dmg, minHP, maxHP);

            if (!currentHP.Equals(minHP))
            {
                return;
            }

            RaiseDeathEvent();
        }

        [Server]
        public void HealPercentageHealth(float percentage)
        {
            currentHP  = Mathf.Clamp(currentHP + Mathf.FloorToInt(maxHP*percentage), minHP, maxHP);
        }


        [Server]
        public void TakeDamage(NetworkIdentity source, int dmg, bool isCritical)
        {
            currentHP = Mathf.Clamp(currentHP - dmg, minHP, maxHP);
            TargetOnDamageGiven(source.connectionToClient, dmg, isCritical);
            TargetOnDamageTaken((transform.position - source.transform.position).normalized);

            if (!currentHP.Equals(minHP))
            {
                return;
            }

            RaiseDeathEvent();    
        }

        [TargetRpc]
        private void TargetOnDamageGiven(NetworkConnection atker, int dmg, bool isCritical)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new DamageGivenData(transform.position, dmg, isCritical));
        }

        [TargetRpc]
        private void TargetOnDamageTaken(Vector2 atkDir) => EventSystems.EventManager.Instance.TriggerEvent(new DamageTakenData(netId, atkDir));

        protected virtual void RaiseDeathEvent() => EventSystems.EventManager.Instance.TriggerEvent(new CharacterDeathData(netId));

        private void OnCurrentHPSync(int oldCurHP, int newCurHP)
        {
            if (oldCurHP > newCurHP)
                damagedVFX.Play();
            else
                PlayHealEffect();

            if (hasAuthority)
            {
                OnAuthorityCurrentHPSync(oldCurHP, newCurHP);
            }
        }

        protected virtual void OnAuthorityCurrentHPSync(int oldCurHP, int newCurHP)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new HPChangeData(oldCurHP, newCurHP, maxHP));
        }

        protected virtual void PlayHealEffect()
        {
            if (HealEffect)
                HealEffect.Play();
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
        void CmdTakeDamage() => TakeDamage(netIdentity, 40, true);
    #endif
    #endregion
    }
}