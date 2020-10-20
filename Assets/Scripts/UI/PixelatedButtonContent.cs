using UnityEngine;

namespace MD.UI
{
    public class PixelatedButtonContent : MonoBehaviour
    {
        [SerializeField]
        private float offset = 0f;

        [SerializeField]
        private PixelatedButton button = null;

        private Vector3 offsetVect;

        private bool isPressed;

        void Start()
        {
            offsetVect = new Vector3(0f, offset, 0f);
            button.OnPress += Lower;
            button.OnRelease += Raise;
            isPressed = false;
        }

        public void Lower()
        {
            if (isPressed) return;

            gameObject.transform.position -= offsetVect;
            isPressed = true;
        }

        public void Raise()
        {
            isPressed = false;
            gameObject.transform.position += offsetVect;
        }
    }
}