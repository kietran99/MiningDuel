using UnityEngine;

namespace MD.Character
{
    public class PickaxeAnimatorController : MonoBehaviour
    {
        protected readonly string SWING = "Swing";
        protected readonly string INTERRUPT = "Interrupt";

        [SerializeField]
        protected Animator animator = null;

        public bool IsAnimPlaying { get; set; } = false;

        protected virtual void Start()
        {
            EventSystems.EventConsumer.Attach(gameObject).StartListening<GetCounteredData>(CancelAnim);
        }

        protected virtual void CancelAnim(GetCounteredData _)
        {
            animator.SetTrigger(INTERRUPT);
        }
        
        public void Play()
        {
            animator.SetTrigger(SWING);
            IsAnimPlaying = true;
        }

        // Ref by Swing animation
        public void OnAnimEnd() => IsAnimPlaying = false;
    }
}
