using UnityEngine;

namespace MD.AI.TheWarden
{
    public class SimWardenParticleController : MonoBehaviour, IWardenParticleController
    {
        [SerializeField]
        private ParticleSystem chaseParticles = null, attackParticles = null;

        public void HideChaseEffect()
        {
            chaseParticles.Stop();
        }

        public void PlayAttackEffect()
        {
            attackParticles.Play();
        }

        public void PlayChaseEffect(Vector2 targetDir)
        {
            chaseParticles.transform.Rotate(new Vector3(0f, 0f, Vector2.SignedAngle(-chaseParticles.transform.right, targetDir)));
            chaseParticles.Play();
        }
    }
}
