using UnityEngine;
using Mirror;
using System;

namespace MD.AI.TheWarden
{
    public class DefaultWardenDamagable : NetworkBehaviour, IWardenDamagable
    {
        public Action<uint> OnDeath { get; set; }

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
