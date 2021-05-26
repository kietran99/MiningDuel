using MD.Character;
using UnityEngine;

namespace MD.VisualEffects
{
    public class PlayerStatusIcon : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer = null;

        [SerializeField]
        private Sprite attackableIcon = null;

        [SerializeField]
        private Sprite stunIcon = null;

        private void Start()
        {
            var eventConsumer = EventSystems.EventConsumer.Attach(gameObject);
            eventConsumer.StartListening<MainActionToggleData>(OnMainActionToggle);
            eventConsumer.StartListening<StunStatusData>(OnStunned);
        }

        private void OnMainActionToggle(MainActionToggleData data)
        {
            spriteRenderer.sprite = data.actionType.Equals(MainActionType.ATTACK) ? attackableIcon : null;
        }

        private void OnStunned(StunStatusData data)
        {
            spriteRenderer.sprite = data.isStunned ? stunIcon : null;
        }
    }
}
