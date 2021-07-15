using UnityEngine;
using UnityEngine.EventSystems;

namespace MD.UI
{
    public class JoystickEnabler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField]
        private Joystick joystick = null;

        public void OnPointerDown(PointerEventData eventData)
        {
            joystick.gameObject.SetActive(true);
            joystick.transform.position = eventData.pressPosition;
            joystick.OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            joystick.InputDirection = Vector2.zero;
            joystick.gameObject.SetActive(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            joystick.OnDrag(eventData);
        }
    }
}
