using MD.Diggable.Projectile;
using MD.UI;
using Mirror;
using UnityEngine;

namespace MD.Character
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorController : MonoBehaviour
    {
        [SerializeField]
        private NetworkAnimator networkAnimator = null;

        private Animator animator;

        private float lastX, lastY;
        [SerializeField]

        private bool isStunned = false;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            if (!networkAnimator.isLocalPlayer) return;

            var eventManager = EventSystems.EventManager.Instance;
            eventManager.StartListening<JoystickDragData>(SetMovementState);
            eventManager.StartListening<DigInvokeData>(InvokeDig);
            eventManager.StartListening<ProjectileObtainData>(SetHoldState);
            eventManager.StartListening<ThrowInvokeData>(RevertToIdleState);
            var eventConsumer = EventSystems.EventConsumer.Attach(gameObject);
            eventConsumer.StartListening<AttackDirectionData>(PlayBasicAttack);
            eventConsumer.StartListening<StunStatusData>(HandleStunStatusChange);
        }

        private void OnDisable()
        {
            if (!networkAnimator.isLocalPlayer) return;

            var eventManager = EventSystems.EventManager.Instance;
            eventManager.StopListening<JoystickDragData>(SetMovementState);
            eventManager.StopListening<DigInvokeData>(InvokeDig);
            eventManager.StopListening<ProjectileObtainData>(SetHoldState);
            eventManager.StopListening<ThrowInvokeData>(RevertToIdleState);
        }

        private void HandleStunStatusChange(StunStatusData data)
        {
            isStunned = data.isStunned;
        }

        private void RevertToIdleState(ThrowInvokeData obj)
        {
            animator.SetBool(AnimatorConstants.IS_HOLDING, false);
        }

        private void SetHoldState(ProjectileObtainData data)
        {
            if (data.type.Equals(DiggableType.NORMAL_BOMB))
            {
                animator.SetBool(AnimatorConstants.IS_HOLDING, true);
            }
        }

        private void InvokeDig(DigInvokeData obj)
        {
            if (isStunned) return;
            animator.SetBool(AnimatorConstants.IS_DIGGING, true); 
        }

        private void SetMovementState(JoystickDragData dragData)
        {
            if (isStunned) 
            {
                animator.SetFloat(AnimatorConstants.SPEED, 0f);
                PlayIdle();  
                return;
            }
            
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

        private void PlayBasicAttack(AttackDirectionData data)
        {
            animator.SetFloat(AnimatorConstants.ATK_X, data.dir.x);
            animator.SetFloat(AnimatorConstants.ATK_Y, data.dir.y);
            networkAnimator.SetTrigger(AnimatorConstants.BASIC_ATTACK);
        }
    }
}