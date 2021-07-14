using UnityEngine;

namespace Utils.VFX
{
    public class FloatingEffect : MonoBehaviour
    {
        [SerializeField]
        private float offset = 10f;

        [SerializeField]
        private float moveSpeed = 10f;

        private Vector2 originalPos;
        private float maxY, minY;
        private bool shouldMoveUp = true;

        private void Start()
        {
            originalPos = transform.localPosition;
            maxY = transform.localPosition.y + offset;
            minY = transform.localPosition.y - offset;
        }

        public void SetPlayState(bool shouldPlay)
        {
            if (shouldPlay)
            {
                Play();
                return;
            }

            Stop();
        }

        private void Play() => enabled = true;

        private void Stop()
        {
            transform.localPosition = originalPos;
            enabled = false;
        }    

        private void Update()
        {
            if (shouldMoveUp)
            {            
                transform.localPosition += new Vector3(0f, moveSpeed * Time.deltaTime, 0f);

                if (transform.localPosition.y >= maxY)
                {
                    shouldMoveUp = false;
                }

                return;
            }
           
            transform.localPosition -= new Vector3(0f, moveSpeed * Time.deltaTime, 0f);

            if (transform.localPosition.y <= minY)
            {
                shouldMoveUp = true;
            }
        }
    }
}
