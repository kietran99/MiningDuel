using MD.Character;
using MD.UI;
using UnityEngine;

namespace MD.VisualEffects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ThrowChargeIndicator : MonoBehaviour
    {
        [SerializeField]
        private Player player = null;

        [SerializeField]
        private float showPeriod = .5f;

        private SpriteRenderer spriteRenderer;

        void Start()
        {           
            if (!player.isLocalPlayer) return;
            
            spriteRenderer = GetComponent<SpriteRenderer>();
            var eventConsumer = gameObject.AddComponent<EventSystems.EventConsumer>();
            eventConsumer.StartListening<UI.ThrowInvokeData>(Show);        
        }

        private void Show(ThrowInvokeData _)
        {
            spriteRenderer.enabled = true;
            Invoke(nameof(Hide), showPeriod);
        }

        private void Hide() => spriteRenderer.enabled = false;
    }
}
