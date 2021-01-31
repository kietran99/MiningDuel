using MD.Diggable.Projectile;
using MD.UI;
using UnityEngine;
using Mirror;

namespace MD.Character
{
    public class ThrowAction : NetworkBehaviour
    {
        [SerializeField]
        protected float basePower = 100f;
        
        private Vector2 currentDir = Vector2.zero;
        protected ProjectileLauncher holdingProjectile;

        public override void OnStartAuthority()
        {
            EventSystems.EventManager.Instance.StartListening<TargetedThrowInvokeData>(HandleTargetedThrowInvokeData);
        }

        public override void OnStopAuthority()
        {
            EventSystems.EventManager.Instance.StopListening<TargetedThrowInvokeData>(HandleTargetedThrowInvokeData);
        }

        private void HandleTargetedThrowInvokeData(TargetedThrowInvokeData targetedThrowInvokeData)
        {
            var normalizedThrowDirection = 
                new Vector2(targetedThrowInvokeData.x - transform.position.x, targetedThrowInvokeData.y - transform.position.y).normalized;
            CmdThrowProjectile(normalizedThrowDirection.x, normalizedThrowDirection.y, basePower);
        }

        private void BindThrowDirection(JoystickDragData dragData)
        {
            if (dragData.InputDirection == Vector2.zero) return;
            currentDir = dragData.InputDirection;
        }

        private void ThrowProjectile(ThrowInvokeData data)
        {
            CmdThrowProjectile(currentDir.x, currentDir.y, basePower);
        }

        [Command]
        protected void CmdThrowProjectile(float dirX, float dirY, float power)
        {
            if (holdingProjectile == null) 
            {
                Debug.LogWarning("Not Holding Any Projectile");
                return;
            }
            
            holdingProjectile.Launch(basePower, dirX, dirY);
        }

        public virtual void SetHoldingProjectile(ProjectileLauncher proj) => holdingProjectile = proj;
    }
}