using UnityEngine;

namespace MD.UI
{
    public class PixelatedButtonContent : MonoBehaviour
    {
        [SerializeField]
        private float offset = 0f;

        [SerializeField]
        private PixelatedButton button = null;

        private Vector3 originalPos;
        private Vector3 offsetVect;

        //private bool isPressed;

        void Start()
        {
            originalPos = transform.localPosition;
            if (GetComponentInParent<DigControl>() != null) Debug.Log(originalPos);
            offsetVect = new Vector3(0f, offset, 0f);
            button.OnPress += Lower;
            button.OnRelease += Raise;
            //isPressed = false;
        }

        public void Lower()
        {
            //if (isPressed) return;

            transform.localPosition = originalPos - offsetVect;
            //isPressed = true;
        }

        public void Raise()
        {
            //isPressed = false;
            transform.localPosition = originalPos;
        }
    }
}