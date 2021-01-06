using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MD.UI
{
    [RequireComponent(typeof(Button))]
    public class PixelatedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] Sprite unpressedSprite = null, pressedSprite = null;

        public Action OnPress { get; set; }
        public Action OnRelease { get; set; }

        private Button myButton;

        void Awake()
        {
            myButton = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {           
            if (!myButton.interactable) return;

            myButton.image.sprite = pressedSprite;
            OnPress?.Invoke();
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {   
            if (!myButton.interactable) return;

            myButton.image.sprite = unpressedSprite;
            OnRelease?.Invoke(); 
        }
    }
}
