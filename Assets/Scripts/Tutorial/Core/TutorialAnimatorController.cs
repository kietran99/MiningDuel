using MD.Character;
using MD.Diggable.Projectile;
using MD.UI;
using UnityEngine;

namespace MD.Tutorial
{
    [RequireComponent(typeof(Animator))]
    public class TutorialAnimatorController : MonoBehaviour
    {
        private Animator animator;

        private float lastX, lastY;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            var eventConsumer = gameObject.AddComponent<EventSystems.EventConsumer>();
            eventConsumer.StartListening<JoystickDragData>(SetMovementState);
            eventConsumer.StartListening<DigInvokeData>(InvokeDig);
            eventConsumer.StartListening<ProjectileObtainData>(SetHoldState);
            eventConsumer.StartListening<ThrowInvokeData>(RevertToIdleState);
        }

        private void RevertToIdleState(ThrowInvokeData obj)
        {
            animator.SetBool(AnimatorConstants.IS_HOLDING, false);
        }

        private void SetHoldState(ProjectileObtainData obj)
        {
            animator.SetBool(AnimatorConstants.IS_HOLDING, true);
        }

        private void InvokeDig(DigInvokeData obj)
        {
            animator.SetBool(AnimatorConstants.IS_DIGGING, true); 
        }

        private void SetMovementState(JoystickDragData dragData)
        {
            var speed = dragData.InputDirection.sqrMagnitude;
            animator.SetFloat(AnimatorConstants.HORIZONTAL, dragData.InputDirection.x);
            animator.SetFloat(AnimatorConstants.VERTICAL, dragData.InputDirection.y);
            animator.SetFloat(AnimatorConstants.SPEED, speed);           

            if (speed.IsEqual(0f))
            {
                PlayIdle();
                return;
            }

            BindLastMoveStats(dragData.InputDirection.x, dragData.InputDirection.y);
            CancelDigAction();
        }

        private void PlayIdle()
        {
            animator.SetFloat(AnimatorConstants.LAST_X, lastX);
            animator.SetFloat(AnimatorConstants.LAST_Y, lastY);
        }

        private void BindLastMoveStats(float lastX, float lastY) => (this.lastX, this.lastY) = (lastX, lastY);    

        public void CancelDigAction() => animator.SetBool(AnimatorConstants.IS_DIGGING, false);
    }
}
