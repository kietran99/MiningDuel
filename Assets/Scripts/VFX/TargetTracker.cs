using UnityEngine;

namespace MD.VisualEffects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TargetTracker : MonoBehaviour
    {
        private Transform targetTransform;
        private SpriteRenderer spriteRenderer;

        public Vector2 targetPos => targetTransform.position;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void FixedUpdate()
        {
            if (targetTransform == null) return;

            transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y, transform.position.z);
        }

        public void StartTracking(Transform targetTransform)
        {
            this.targetTransform = targetTransform;
            spriteRenderer.enabled = true;
        } 

        public void StopTracking() 
        {
            targetTransform = null;
            spriteRenderer.enabled = false;
        }
    }
}