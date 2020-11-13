using MD.Character;
using UnityEngine;

namespace MD.VisualEffects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ThrowDirectionVisual : MonoBehaviour
    {
        [SerializeField]
        private Player player;
        private Renderer spriteRenderer;
        private float distanceFromPlayer;

        private Vector2 currentDir = Vector2.zero;

        void Start()
        {            
            if (!player.isLocalPlayer) return; 
            
            spriteRenderer = GetComponent<SpriteRenderer>();
            distanceFromPlayer = transform.localPosition.magnitude;

            ListenToEvents();
        }

        void OnDisable() 
        {
            if (player == null) return;

            if (!player.isLocalPlayer) return; 

            var eventManager = EventSystems.EventManager.Instance; 
            eventManager.StopListening<Diggable.Projectile.ProjectileObtainData>(Show);
            eventManager.StopListening<UI.JoystickDragData>(OnCharacterMove);
            eventManager.StopListening<ThrowInvokeData>(Hide);
        }
        
        private void ListenToEvents()
        {
            var eventManager = EventSystems.EventManager.Instance; 
            eventManager.StartListening<Diggable.Projectile.ProjectileObtainData>(Show);
            eventManager.StartListening<UI.JoystickDragData>(OnCharacterMove);
            eventManager.StartListening<ThrowInvokeData>(Hide);
        }

        public void Hide(ThrowInvokeData obj)
        {
            spriteRenderer.enabled = false;
        } 

        public void Show(Diggable.Projectile.ProjectileObtainData obj)
        {       
            Debug.Log("Picked up a projectile");     
            spriteRenderer.enabled = true;
            Rotate();
        } 

        private void OnCharacterMove(UI.JoystickDragData joystickData)
        {
            if (joystickData.InputDirection != Vector2.zero)
            {
                currentDir = joystickData.InputDirection;
                Rotate();
            }

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
