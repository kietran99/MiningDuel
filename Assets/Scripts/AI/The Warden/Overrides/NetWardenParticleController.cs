using UnityEngine;
using Mirror;

namespace MD.AI.TheWarden
{
    public class NetWardenParticleController : NetworkBehaviour, IWardenParticleController
    {
        [SerializeField]
        private BaseWardenParticleController baseController = null;

        [Server]        
        public void HideChaseEffect() => RpcHideChaseEffect();

        [Server]
        public void HideWanderEffect() => RpcHideWanderEffect();

        [Server]
        public void PlayAttackEffect() => RpcPlayAttackEffect();

        [Server]
        public void PlayChaseEffect(Vector2 targetDir) => RpcPlayChaseEffect(targetDir);

        [Server]
        public void PlayWanderEffect(Vector2 targetDir) => RpcPlayWanderEffect(targetDir);

        [ClientRpc]
        private void RpcHideChaseEffect() => baseController.HideChaseEffect();    

        [ClientRpc]
        private void RpcPlayAttackEffect() => baseController.PlayAttackEffect();

        [ClientRpc]
        private void RpcPlayChaseEffect(Vector2 targetDir) => baseController.PlayChaseEffect(targetDir);

        [ClientRpc]    
        private void RpcPlayWanderEffect(Vector2 targetDir) => baseController.PlayWanderEffect(targetDir);
    
        [ClientRpc]
        private void RpcHideWanderEffect() => baseController.HideWanderEffect();
    }
}
