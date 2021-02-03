using UnityEngine;

namespace MD.Character
{
    public struct NonTargetError : Functional.IError
    {
        public string Message => "No Target is being Tracked";
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public class TargetTracker : MonoBehaviour
    {
        private Transform targetTransform;
        private SpriteRenderer spriteRenderer;

        // public Functional.Type.Either<NonTargetError, Vector3> TargetPosition 
        // {
        //     get
        //     {
        //         if (targetTransform == null)
        //         {
        //             return new NonTargetError();
        //         }

        //         return targetTransform.position;
        //     }
        // }

        public Vector2 TargetPosition => targetTransform.position;

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