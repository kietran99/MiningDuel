﻿using MD.Diggable.Projectile;
using MD.UI;
using UnityEngine;

namespace MD.Character
{
    public class ThrowAction : MonoBehaviour
    {
        [SerializeField]
        private float basePower = 100f;

        private GameObject projectile;

        void Start()
        {
            EventSystems.EventManager.Instance.StartListening<ThrowInvokeData>(ThrowProjectile);
            EventSystems.EventManager.Instance.StartListening<JoystickDragData>(BindThrowDirection);
        }

        void OnDestroy()
        {
            EventSystems.EventManager.Instance.StopListening<ThrowInvokeData>(ThrowProjectile);
            EventSystems.EventManager.Instance.StopListening<JoystickDragData>(BindThrowDirection);
        }

        private void BindThrowDirection(JoystickDragData dragData)
        {
            if (projectile == null) return;

            projectile.GetComponent<ProjectileLauncher>().BindThrowDirection(
                new Vector2(dragData.InputDirection.x, dragData.InputDirection.y));
        }

        public void BindProjectile(GameObject projectile) => this.projectile = projectile;

        private void ThrowProjectile(ThrowInvokeData data)
        {
            projectile.GetComponent<ProjectileLauncher>().Launch(basePower);
            projectile = null;
        }
    }
}