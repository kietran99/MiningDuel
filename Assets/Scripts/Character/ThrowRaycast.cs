using MD.Diggable.Projectile;
using MD.UI;
using UnityEngine;

namespace MD.Character
{
    [RequireComponent(typeof(ThrowAction))]
    public class ThrowRaycast : Mirror.NetworkBehaviour
    {       
        private bool shouldRaycast = false;
        private ThrowAction throwAction;

        public override void OnStartLocalPlayer()
        {
            throwAction = GetComponent<ThrowAction>();
            var eventConsumer = gameObject.AddComponent<EventSystems.EventConsumer>();
            eventConsumer.StartListening<Diggable.Projectile.ProjectileObtainData>(EnableRaycast);
            eventConsumer.StartListening<UI.JoystickDragData>(StartRaycastingPlayers);
            eventConsumer.StartListening<UI.ThrowInvokeData>(DisableRaycast);              
        }

        private void EnableRaycast(ProjectileObtainData _) => shouldRaycast = true;

        private void DisableRaycast(ThrowInvokeData _) => shouldRaycast = false;

        private void StartRaycastingPlayers(JoystickDragData joystickDragData)
        {
            if (!shouldRaycast) 
            {
                return;
            }

            // Debug.DrawLine(transform.position, joystickDragData.InputDirection * 200f);
            var hit = Physics2D.Raycast(transform.position, joystickDragData.InputDirection, Mathf.Infinity);
            var target = hit.collider;

            if (target == null) 
            {
                return;
            }

            if (!target.CompareTag(Constants.PLAYER_TAG))
            {
                return;
            }

            throwAction.StartTracking(target.transform);
        }        
    }
}
