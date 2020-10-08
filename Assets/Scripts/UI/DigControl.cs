using System;
using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    [RequireComponent (typeof(Button))]
    public class DigControl : MonoBehaviour
    {
        private Button button;

        public static Action digButtonClick;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Invoke);
        }

        public void Invoke()
        {
            digButtonClick?.Invoke();
        }
    }
}
