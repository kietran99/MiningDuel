using UnityEngine;

namespace MD.VisualEffects
{
    [RequireComponent(typeof(ParticleSystem))]
    public class MovementDustEffectController : MonoBehaviour
    {
        private ParticleSystem dustEffect = null;

        private void Start()
        {
            dustEffect = GetComponent<ParticleSystem>();
            gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<UI.JoystickDragData>(ToggleEffect);
        }

        private void ToggleEffect(UI.JoystickDragData joystickDragData)
        {
            if (joystickDragData.InputDirection.Equals(Vector2.zero))
            {
                dustEffect.Stop();
                return;
            }
           
            if (!dustEffect.isPlaying)
            {
                dustEffect.Play();
            }            
        }
    }
}
