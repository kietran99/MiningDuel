using UnityEngine;

namespace MD.Character
{
    public class PickaxeAnimatorController : MonoBehaviour
    {
        [SerializeField]
        private Animator animator = null;

        public void Play()
        {
            animator.SetTrigger(AnimatorConstants.SWING);
        }
    }
}
