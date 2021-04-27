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

        [Server]
        public void TakeDamage(int dmg)
        {
            currentHP = Mathf.Clamp(currentHP - dmg, minHP, maxHP);
        }

        private void OnCurrentHPSync(int oldCurHP, int newCurHP)
        {
            if (hasAuthority)
            {
                OnAuthorityCurrentHPSync(oldCurHP, newCurHP);
            }
        }

        protected virtual void OnAuthorityCurrentHPSync(int oldCurHP, int newCurHP)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new HPChangeData(newCurHP, maxHP));
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

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                CmdTakeDamage();
            }
        }

        [Command]
        void CmdTakeDamage() => TakeDamage(20);
    #endif
    #endregion
    }
}