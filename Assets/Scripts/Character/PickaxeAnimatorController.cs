using Mirror;
using UnityEngine;

namespace MD.Character
{
    public class PickaxeAnimatorController : MonoBehaviour
    {
        [SerializeField]
        private Animator animator = null;

        public System.Action<NetworkIdentity> OnDamagableEnter { get; set; }
        public System.Action<NetworkIdentity> OnDamagableExit { get; set; }

        public void Play(Vector2 targetDir)
        {
            transform.Rotate(0f, 0f, Vector2.SignedAngle(-transform.up, targetDir));
            animator.SetTrigger(AnimatorConstants.SWING);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var damagable = other.GetComponent<IDamagable>();

            if (damagable == null)
            {
                return;
            }

            OnDamagableEnter?.Invoke(other.GetComponent<NetworkIdentity>());
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var damagable = other.GetComponent<IDamagable>();

            if (damagable == null)
            {
                return;
            }

            OnDamagableExit?.Invoke(other.GetComponent<NetworkIdentity>());
        }
    }
}
