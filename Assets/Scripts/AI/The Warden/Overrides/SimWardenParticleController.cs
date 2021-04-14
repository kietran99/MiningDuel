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

        public void PlayChaseEffect()
        {
            chaseParticles.Play();
        }
    }
}
