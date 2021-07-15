using UnityEngine;

namespace MD.VisualEffects
{
    public class ExplosionEffect : MonoBehaviour
    {
        [SerializeField]
        private Animator animator = null;

        private readonly string PLAY = "Play";

        private System.Action endCallback;

        public void Play(System.Action endCallback)
        {
            this.endCallback = endCallback;
            animator.SetTrigger(PLAY);
        }

        public void OnAnimEnd() // Referenced by animation clip
        {
            endCallback?.Invoke();
        }
    }
}
