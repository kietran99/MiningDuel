using MD.Diggable.Projectile;
using UnityEngine;
using Mirror;
using MD.UI;
using UnityEngine.EventSystems;

namespace MD.Character
{
    public class ThrowAction : NetworkBehaviour
    {
        #region SERIALIZED FIELDS
        [SerializeField]
        private ThrowChargeIndicator throwChargeIndicator = null;

        [SerializeField]
        protected float basePower = 100f;

        [SerializeField]
        protected float chargeTime = .5f;
        #endregion

        #region FIELDS
        protected ProjectileLauncher holdingProjectile;
        protected WaitForSecondsRealtime chargeTimeWFS;  
        private Camera mainCamera;
        private bool raycastedUI;
        #endregion

        public override void OnStartLocalPlayer()
        {   
            chargeTimeWFS = new WaitForSecondsRealtime(chargeTime);   
        }

        [ClientCallback]
        private void Update()
        {     
            if (!isLocalPlayer)
            {
                return;
            }

            if (mainCamera == null) mainCamera = Camera.main;

            #if UNITY_EDITOR
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            #elif UNITY_ANDROID
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }
            #endif

            if (holdingProjectile == null) 
            {
                // Debug.LogError("Not Holding Any Projectile");
                return;
            }

            if (!Input.GetMouseButtonDown(0))
            {
                return; 
            }

            Vector2 clickPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 throwDir = clickPos - new Vector2(transform.position.x, transform.position.y);
            HandleThrowCommand(throwDir.normalized);
            EventSystems.EventManager.Instance.TriggerEvent(new ThrowInvokeData());
        }

        private void HandleThrowCommand(Vector2 normalizedThrowDir)
        {
            CmdShowIndicatorMomentarily();
            StartCoroutine(ChargedThrow(normalizedThrowDir));
        }

        [Command]
        private void CmdShowIndicatorMomentarily() 
        {
            RpcShowThrowChargeIndicator();
            Invoke(nameof(RpcHideThrowChargeIndicator), chargeTime);
        }

        [ClientRpc]
        private void RpcShowThrowChargeIndicator() => throwChargeIndicator.Show();

        [ClientRpc]
        private void RpcHideThrowChargeIndicator() => throwChargeIndicator.Hide();

        [Server]
        public virtual void SetHoldingProjectile(ProjectileLauncher proj) => holdingProjectile = proj;

        private System.Collections.IEnumerator ChargedThrow(Vector2 throwDirection)
        {
            yield return chargeTimeWFS;

            CmdThrow(throwDirection.x, throwDirection.y);
        }

        [Command]
        protected void CmdThrow(float dirX, float dirY)
        {
            holdingProjectile.Launch(basePower, dirX, dirY);
            holdingProjectile = null;
        }
    }
}