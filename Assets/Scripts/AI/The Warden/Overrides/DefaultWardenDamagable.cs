using UnityEngine;
using Mirror;

namespace MD.AI.TheWarden
{
    public class DefaultWardenDamagable : NetworkBehaviour, IWardenDamagable
    {
        [Server]
        public void TakeWardenDamage(int dmg)
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
