using UnityEngine;

namespace MD.Tutorial
{
    public class FloatingEffect : MonoBehaviour
    {
        [SerializeField]
        private float offset = 10f;

        [SerializeField]
        private float moveSpeed = 10f;

        private float maxY, minY;
        private bool shouldMoveUp = true;

        private void Start()
        {
            maxY = transform.position.y + offset;
            minY = transform.position.y - offset;
        }

        private void Update()
        {
            if (shouldMoveUp)
            {            
                transform.position += new Vector3(0f, moveSpeed * Time.deltaTime, 0f);

                if (transform.position.y >= maxY)
                {
                    shouldMoveUp = false;
                }

                return;
            }
           
            transform.position -= new Vector3(0f, moveSpeed * Time.deltaTime, 0f);

            if (transform.position.y <= minY)
            {
                shouldMoveUp = true;
            }
        }
    }
}
