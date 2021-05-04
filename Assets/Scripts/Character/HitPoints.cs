﻿using UnityEngine;
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
        private float knockbackForce = .2f;

        [SerializeField]
        private VisualEffects.DamagedVFX damagedVFX = null;

        public System.Action<uint> OnOutOfHP { get; set; }

        [Server]
        public void TakeDamage(NetworkIdentity source, int dmg)
        {
            currentHP = Mathf.Clamp(currentHP - dmg, minHP, maxHP);
            transform.Translate((transform.position - source.transform.position).normalized * knockbackForce);

            if (!currentHP.Equals(minHP))
            {
                return;
            }

            OnOutOfHP?.Invoke(netId);
        }

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