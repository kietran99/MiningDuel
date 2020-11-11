using MD.Character;
using MD.Diggable;
using UnityEngine;

namespace MD.VisualEffects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ThrowDirectionVisual : MonoBehaviour
    {
        //[SerializeField]
        private Player player;
        private Renderer spriteRenderer;
        private float distanceFromPlayer;

        void Start()
        {
            //if (!ServiceLocator.Resolve(out player)) return;
            //Debug.Log("Found player");

            //if (!player.isLocalPlayer) return; 
            //Debug.Log("Is local player");
            
            // spriteRenderer = GetComponent<SpriteRenderer>();
            // distanceFromPlayer = transform.localPosition.magnitude;

            // ListenToEvents();
        }

        void OnDestroy() 
        {
            if (player == null) return;

            if (!player.isLocalPlayer) return; 

            var eventManager = EventSystems.EventManager.Instance; 
            eventManager.StopListening<Diggable.DiggableDestroyData>(Show);
            eventManager.StopListening<UI.JoystickDragData>(Rotate);
            eventManager.StopListening<ThrowInvokeData>(Hide);
        }

        public void BindPlayer(Player player) 
        {
            this.player = player;

            if (!player.isLocalPlayer) return; 
            Debug.Log("Is local player");

            spriteRenderer = GetComponent<SpriteRenderer>();
            distanceFromPlayer = transform.localPosition.magnitude;

            ListenToEvents();
        } 

        private void ListenToEvents()
        {
            var eventManager = EventSystems.EventManager.Instance; 
            eventManager.StartListening<Diggable.DiggableDestroyData>(Show);
            eventManager.StartListening<UI.JoystickDragData>(Rotate);
            eventManager.StartListening<ThrowInvokeData>(Hide);
        }

        public void Hide(ThrowInvokeData obj)
        {
            spriteRenderer.enabled = false;
        } 

        public void Show(DiggableDestroyData obj)
        {            
            spriteRenderer.enabled = true;
        } 

        public void Rotate(UI.JoystickDragData joystickData)
        {
            if (!spriteRenderer.enabled) return;

            if (joystickData.InputDirection.Equals(Vector2.zero)) return;

            var angle = Mathf.Rad2Deg * Mathf.Atan2(joystickData.InputDirection.y, joystickData.InputDirection.x);
            transform.localEulerAngles = new Vector3(0f, 0f, angle);
            transform.localPosition = new Vector3(distanceFromPlayer * Mathf.Cos(Mathf.Deg2Rad * angle), 
                                                    distanceFromPlayer * Mathf.Sin(Mathf.Deg2Rad * angle), 0f);            
        }   
    }
}
