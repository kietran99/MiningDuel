using UnityEngine;
using MD.Character;

namespace MD.VisualEffects
{
    public class AttackableIcon : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer sprite = null;

        private void Start()
        {
            EventSystems.EventConsumer.Attach(gameObject).StartListening<MainActionToggleData>(ShowOrHide);
        }

        private void ShowOrHide(MainActionToggleData data)
        {
            sprite.enabled = data.actionType.Equals(MainActionType.ATTACK);
        }
    }
}
