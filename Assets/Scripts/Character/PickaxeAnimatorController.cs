using UnityEngine;

namespace MD.Character
{
    public class PickaxeAnimatorController : MonoBehaviour
    {
        [SerializeField]
        private Animator animator = null;

        public bool IsAnimPlaying { get; set; } = false;

        private readonly string SWING = "Swing";

        private readonly string INTERRUPT = "Interrupt";

        private void Start()
        {
            EventSystems.EventConsumer.Attach(gameObject).StartListening<GetCounteredData>(CancelAnim);
        }

        private void CancelAnim(GetCounteredData _)
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
