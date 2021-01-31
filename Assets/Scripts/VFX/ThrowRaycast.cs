using MD.Diggable.Projectile;
using MD.UI;
using UnityEngine;

namespace MD.VisualEffects
{
    public class ThrowRaycast : MonoBehaviour
    {
        [SerializeField]
        private TargetTracker targetTracker = null;

        private Transform playerTransform;
        private bool shouldRaycast = false;

        void Start()
        {
            ServiceLocator
                .Resolve<MD.Character.Player>()
                .Match(
                    err => Debug.Log(err.Message),
                    player => 
                    {
                        playerTransform = player.transform;
                        var eventConsumer = gameObject.AddComponent<EventSystems.EventConsumer>();
                        eventConsumer.StartListening<Diggable.Projectile.ProjectileObtainData>(EnableRaycast);
                        eventConsumer.StartListening<UI.JoystickDragData>(StartRaycastingPlayers);
                        eventConsumer.StartListening<UI.ThrowInvokeData>(DisableRaycast);
                    }
                );            
        }

        private void EnableRaycast(ProjectileObtainData _) => shouldRaycast = true;

        private void DisableRaycast(ThrowInvokeData _) 
        {
            shouldRaycast = false;
            targetTracker.StopTracking();
        }

        private void StartRaycastingPlayers(JoystickDragData joystickDragData)
        {
            if (!shouldRaycast) return;

            Debug.DrawLine(playerTransform.position, joystickDragData.InputDirection * 10f);
            var hit = Physics2D.Raycast(playerTransform.position, joystickDragData.InputDirection, Mathf.Infinity);
            var target = hit.collider;

            if (target == null) 
            {
                return;
            }

            if (!target.CompareTag(Constants.PLAYER_TAG))
            {
                return;
            }

            targetTracker.StartTracking(target.transform);
        }        
    }
}
