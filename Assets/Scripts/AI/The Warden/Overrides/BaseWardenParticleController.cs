using UnityEngine;

namespace MD.AI.TheWarden
{
    public class BaseWardenParticleController : MonoBehaviour, IWardenParticleController
    {
        [SerializeField]
        private ParticleSystem wanderParticles = null, chaseParticles = null, attackParticles = null;

        public void HideChaseEffect()
        {
            chaseParticles.Stop();
        }

        public void HideWanderEffect()
        {
            wanderParticles.Stop();
        }

        public void PlayAttackEffect()
        {
            attackParticles.Play();
        }

        public void PlayChaseEffect(Vector2 targetDir)
        {
            PlayMovementEffect(chaseParticles, targetDir);
        }

        public void PlayWanderEffect(Vector2 targetDir)
        {
            PlayMovementEffect(wanderParticles, targetDir);
        }

        private void PlayMovementEffect(ParticleSystem particles, Vector2 targetDir)
        {
            TurnToTarget(particles.transform, targetDir);
            if (!particles.isPlaying)
            {
                particles.Play();
            }
        }
            
        private void TurnToTarget(Transform particles, Vector2 targetDir)
        {
            particles.transform.Rotate(new Vector3(0f, 0f, Vector2.SignedAngle(-particles.transform.right, targetDir)));
        } 
    }
}
