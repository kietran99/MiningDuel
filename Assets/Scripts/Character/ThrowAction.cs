﻿using MD.Diggable.Projectile;
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

        void Start()
        {
            if (!isLocalPlayer) return;

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