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
            EventSystems.EventConsumer.Attach(gameObject).StartListening<Character.AttackTargetChangeData>(ToggleMode);
        }

        private void FixedUpdate()
        {
            transform.position = targetPos;
        }

        private void ToggleMode(Character.AttackTargetChangeData data)
        {
            if (!data.attackable)
            {
                animator.enabled = false;
                spriteRenderer.sprite = null;
                return;
            }

            targetPos = data.targetPos;
            animator.enabled = true;
        }
    }
}
