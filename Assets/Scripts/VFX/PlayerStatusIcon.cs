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
        private Sprite unattackableIcon = null;

        [SerializeField]
        private Sprite stunIcon = null;

        [SerializeField]
        private Utils.VFX.FloatingEffect _floatingEffect = null;

        private bool _attackable;

        private void Start()
        {
            _attackable = true;
            var eventConsumer = EventSystems.EventConsumer.Attach(gameObject);
            eventConsumer.StartListening<MainActionToggleData>(OnMainActionToggle);
            eventConsumer.StartListening<StunStatusData>(OnStunned);
            eventConsumer.StartListening<AttackCooldownData>(OnAtkCooldown);
        }

        private void OnAtkCooldown(AttackCooldownData data)
        {
            _attackable = data.attackable;

            _floatingEffect?.SetPlayState(_attackable);
       
            spriteRenderer.sprite = 
                spriteRenderer.sprite == attackableIcon || spriteRenderer.sprite == unattackableIcon
                ? (data.attackable ? attackableIcon : unattackableIcon)
                : spriteRenderer.sprite;               
        }

        private void OnMainActionToggle(MainActionToggleData data)
        {
            spriteRenderer.sprite = 
                data.actionType == MainActionType.ATTACK 
                ? (_attackable ? attackableIcon : unattackableIcon) 
                : null;
        }

        private void OnStunned(StunStatusData data)
        {
            spriteRenderer.sprite = data.isStunned ? stunIcon : null;
        }
    }
}
