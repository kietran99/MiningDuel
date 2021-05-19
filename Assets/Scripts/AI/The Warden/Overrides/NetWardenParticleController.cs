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
        public void PlayChaseEffect(Vector2 targetDir)
        {
            RpcPlayChaseEffect(targetDir);
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
        public void RpcPlayChaseEffect(Vector2 targetDir)
        {
            chaseParticles.transform.Rotate(new Vector3(0f, 0f, Vector2.SignedAngle(-chaseParticles.transform.right, targetDir)));
            chaseParticles.Play();
        }
    }
}
