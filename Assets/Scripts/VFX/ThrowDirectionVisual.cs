using UnityEngine;

namespace MD.VisualEffects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ThrowDirectionVisual : MonoBehaviour
    {
        private Renderer spriteRenderer;
        private float distanceFromPlayer;
        private Vector2 currentDir = Vector2.zero;

        void Start()
        {            
            spriteRenderer = GetComponent<SpriteRenderer>();
            distanceFromPlayer = transform.localPosition.magnitude;
            ListenToEvents();
        }

        private void ListenToEvents()
        {
            var eventManager = EventSystems.EventManager.Instance; 
            eventManager.StartListening<Diggable.Projectile.ProjectileObtainData>(Show);
            eventManager.StartListening<UI.JoystickDragData>(HandleJoystickDragEvent);
            eventManager.StartListening<UI.ThrowInvokeData>(Hide);
        }

        void OnDisable() 
        {           
            var eventManager = EventSystems.EventManager.Instance; 
            eventManager.StopListening<Diggable.Projectile.ProjectileObtainData>(Show);
            eventManager.StopListening<UI.JoystickDragData>(HandleJoystickDragEvent);
            eventManager.StopListening<UI.ThrowInvokeData>(Hide);
        }

        private void Hide(UI.ThrowInvokeData obj)
        {
            spriteRenderer.enabled = false;
        } 

        private void Show(Diggable.Projectile.ProjectileObtainData obj)
        {       
            spriteRenderer.enabled = true;
            Rotate();
        } 

        private void HandleJoystickDragEvent(UI.JoystickDragData joystickData)
        {
            if (joystickData.InputDirection.Equals(Vector2.zero))
            {
                return;
            }

            currentDir = joystickData.InputDirection;
            Rotate();
        }

        public void Rotate()
        {
            if (!spriteRenderer.enabled) return;

            var angle = Mathf.Rad2Deg * Mathf.Atan2(currentDir.y, currentDir.x);
            transform.localEulerAngles = new Vector3(0f, 0f, angle);
            transform.localPosition = new Vector3(distanceFromPlayer * Mathf.Cos(Mathf.Deg2Rad * angle), 
                                                    distanceFromPlayer * Mathf.Sin(Mathf.Deg2Rad * angle), 0f);            
        }   
    }
}
