using UnityEngine;
using Mirror;

namespace MD.AI.TheWarden
{
    public class DefaultWardenDamagable : NetworkBehaviour, IWardenDamagable
    {
        [Server]
        public void TakeDamage()
        {
            RpcTakeDamage();
        }

        [ClientRpc]
        private void RpcTakeDamage()
        {
            Debug.Log("Damagable Client: " + netId);
        }
    }
}
