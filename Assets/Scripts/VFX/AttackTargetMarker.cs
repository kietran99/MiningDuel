using MD.Character;
using UnityEngine;

namespace MD.VisualEffects
{
    public class AttackTargetMarker : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer = null;

        [SerializeField]
        private Animator animator = null;

        private Vector2 targetPos;

        private void Start()
        {
            var eventConsumer = EventSystems.EventConsumer.Attach(gameObject);
            eventConsumer.StartListening<Character.AttackTargetChangeData>(ToggleMode);
            eventConsumer.StartListening<MainActionToggleData>(OnMainActionToggle);
        }

        private void LateUpdate()
        {
            transform.position = targetPos;
        }

        private void OnMainActionToggle(MainActionToggleData data)
        {
            var isPickaxeActive = data.actionType.Equals(MainActionType.DIG) || data.actionType.Equals(MainActionType.ATTACK);
            
            if (!isPickaxeActive)
            {
                Hide();
            }
        }

        private void ToggleMode(Character.AttackTargetChangeData data)
        {
            if (!data.attackable)
            {
                Hide();
                return;
            }

            targetPos = data.targetPos;
            animator.enabled = true;
        }

        private void Hide()
        {
            animator.enabled = false;
            spriteRenderer.sprite = null;
        }
    }
}
