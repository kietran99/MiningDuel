﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MD.UI
{
    [RequireComponent(typeof(Button))]
    public class PixelatedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] Sprite unpressedSprite = null, pressedSprite = null;

        [SerializeField]
        private float cooldown = 0f;

        private bool canBeInvoked;

        public float Cooldown 
        {
            get => cooldown;
            set
            {
                if (value < 0f)
                {
                    Debug.LogError("Cooldown value must be non-negative");
                    return;
                }

                cooldown = value;
            }
        }

        public Action OnPress { get; set; }
        public Action OnRelease { get; set; }

        private Button myButton;

        void Awake()
        {
            myButton = GetComponent<Button>();
        }

        void OnEnable()
        {
            myButton.interactable = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //if (!canBeInvoked) return;

            if (!myButton.interactable) return;

            myButton.image.sprite = pressedSprite;
            OnPress?.Invoke();
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            //if (!canBeInvoked) return;

            if (!myButton.interactable) return;
           
            if (cooldown == 0f) 
            {
                PopButtonUp();
                return;
            }
            
            StartCoroutine(KeepPressing());
            //canBeInvoked = false;
            //Invoke(nameof(PopButtonUp), cooldown);
        }

        private System.Collections.IEnumerator KeepPressing()
        {
            // Execute on the next frame after OnClick method on Button has executed
            yield return null;
            myButton.interactable = false;

            yield return new WaitForSecondsRealtime(cooldown);
            myButton.interactable = true;
            PopButtonUp();
        }

        private void PopButtonUp()
        {
            //Debug.Log("Pop");
            //canBeInvoked = true;
            myButton.image.sprite = unpressedSprite;
            OnRelease?.Invoke(); 
        }
    }
}
