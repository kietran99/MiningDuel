using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MD.UI
{
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {        
        [SerializeField]
        private float offset = 1f;

        private Vector2 inputDirection;
        private Image backgroundImage, joystickImage;

        private bool firstStop = true;

        public Vector2 InputDirection
        {
            get => inputDirection;
            set
            {
                inputDirection = value;
                joystickImage.rectTransform.anchoredPosition = inputDirection;   

                if (!inputDirection.Equals(Vector2.zero)) 
                {
                    firstStop = true;
                    EventSystems.EventManager.Instance.TriggerEvent(new JoystickDragData(value.normalized));
                    return;
                }
                
                if (!firstStop) return;

                firstStop = false;
                EventSystems.EventManager.Instance.TriggerEvent(new JoystickDragData(value.normalized));             
            }
        }

        void Awake()
        {
            backgroundImage = GetComponent<Image>();
            joystickImage = transform.GetChild(0).GetComponent<Image>();
        }

        void Start()
        {
            InputDirection = Vector2.zero;
            offset = offset == 0f ? 1f : offset;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            InputDirection = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            var bgImgSize = backgroundImage.rectTransform.sizeDelta;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundImage.rectTransform,
                eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
            {
                Vector2 fingerPos = new Vector2(localPoint.x / bgImgSize.x, localPoint.y / bgImgSize.y);
                Vector2 movablePos = fingerPos.sqrMagnitude > 1 ? fingerPos.normalized : fingerPos;
                InputDirection = new Vector2(movablePos.x * (bgImgSize.x / offset), movablePos.y * (bgImgSize.y / offset));               
                //Debug.Log("Joystick 8-direction: " + InputDirection.normalized);
            }
        }        
    }
}