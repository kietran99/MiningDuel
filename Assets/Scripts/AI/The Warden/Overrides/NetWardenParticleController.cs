using UnityEngine;
using Mirror;

namespace MD.AI.TheWarden
{
    public class NetWardenParticleController : NetworkBehaviour, IWardenParticleController
    {
        [SerializeField]
        private ParticleSystem chaseParticles = null, attackParticles = null;

        [Server]        
        public void HideChaseEffect()
        {
            RpcHideChaseEffect();
        }

        [Server]
        public void PlayAttackEffect()
        {
            RpcPlayAttackEffect();
        }

        [Server]
        public void PlayChaseEffect()
        {
            RpcPlayChaseEffect();
        }

        [ClientRpc]
        private void RpcHideChaseEffect()
        {
            chaseParticles.Stop();
        }

        [ClientRpc]
        private void RpcPlayAttackEffect()
        {
            attackParticles.Play();
        }

        [ClientRpc]
        private void RpcPlayChaseEffect()
        {
            chaseParticles.Play();
        }
    }
}
