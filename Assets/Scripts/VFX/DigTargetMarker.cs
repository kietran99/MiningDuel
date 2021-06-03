using MD.Character;
using UnityEngine;

namespace MD.VisualEffects
{
    public class DigTargetMarker : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer = null;

        private bool isActive = true;
        private Transform target;

        private void Start()
        {
            target = ServiceLocator.Resolve<Character.Player>().Match(err => transform, player => player.transform);
            EventSystems.EventConsumer.GetOrAttach(gameObject).StartListening<MainActionToggleData>(OnMainActionToggle);
        }

        private void LateUpdate()
        {
            if (!isActive)
            {
                return;
            }

            if (target == null)
            {
                return;
            }
            
            transform.position = new Vector3(Mathf.FloorToInt(target.position.x) + .5f, Mathf.FloorToInt(target.position.y) + .5f, 0f);
        }

        private void OnMainActionToggle(MainActionToggleData data)
        {
            isActive = data.actionType.Equals(MainActionType.DIG);
            spriteRenderer.enabled = isActive;
        }
    }
}
