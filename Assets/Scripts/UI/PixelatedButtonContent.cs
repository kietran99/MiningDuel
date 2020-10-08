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

        void Start()
        {
            offsetVect = new Vector3(0f, offset, 0f);
            button.OnPress += Lower;
            button.OnRelease += Raise;
        }

        public void Lower()
        {
            gameObject.transform.position -= offsetVect;
        }

        public void Raise()
        {
            gameObject.transform.position += offsetVect;
        }
    }
}