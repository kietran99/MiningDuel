using MD.Diggable.Projectile;
using MD.UI;
using UnityEngine;
using Mirror;

namespace MD.Character
{
    public class ThrowAction : NetworkBehaviour
    {
        [SerializeField]
        private float basePower = 100f;
        private Vector2 currentDir = Vector2.zero;
        private ProjectileLauncher holdingProjectile;

        void Start()
        {
            if (!isLocalPlayer) return;
            //Debug.Log("Register throw action");
            EventSystems.EventManager.Instance.StartListening<ThrowInvokeData>(ThrowProjectile);
            EventSystems.EventManager.Instance.StartListening<JoystickDragData>(BindThrowDirection);
        }

        void OnDestroy()
        {
            if (!isLocalPlayer) return;
            EventSystems.EventManager.Instance.StopListening<ThrowInvokeData>(ThrowProjectile);
            EventSystems.EventManager.Instance.StopListening<JoystickDragData>(BindThrowDirection);
        }

        private void BindThrowDirection(JoystickDragData dragData)
        {
            if (dragData.InputDirection == Vector2.zero) return;
            currentDir = dragData.InputDirection;
            // projectile.GetComponent<ProjectileLauncher>().BindThrowDirection(
            //     new Vector2(dragData.InputDirection.x, dragData.InputDirection.y));
        }

        // public void BindProjectile(GameObject projectile) => this.projectile = projectile;
        private void ThrowProjectile(ThrowInvokeData data)
        {
            CmdThrowProjectile(currentDir.x, currentDir.y, basePower);
        }

        [Command]
        private void CmdThrowProjectile(float dirX, float dirY, float power)
        {
            holdingProjectile.Launch(basePower,dirX, dirY);
        }

        public void SetHoldingProjectile(ProjectileLauncher proj) => holdingProjectile = proj;
    }
}