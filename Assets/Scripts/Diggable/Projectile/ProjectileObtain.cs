﻿using UnityEngine;
using Mirror;
using MD.Character;
using EventSystems;

namespace MD.Diggable.Projectile
{
    [RequireComponent(typeof(DiggableProjectile))]
    public class ProjectileObtain : NetworkBehaviour, IDiggable
    {        
        private DigAction currentDigger = null;

        private NetworkIdentity diggerID;
        private Player player = null;
        private Player Player
        {
            get
            {
                if (player != null) return player;
                ServiceLocator.Resolve<Player>(out player);
                return player;
            }
        }

        private DiggableProjectile projectile;
        
        public override void OnStartClient()
        {
            projectile = GetComponent<DiggableProjectile>();
            // EventManager.Instance.TriggerEvent(new DiggableSpawnData(projectile.Type, transform.position.x, transform.position.y));
        }

        public override void OnStopClient()
        {
            //fire an event for sonar to update
            EventManager.Instance.TriggerEvent(new DiggableDestroyData(projectile.Type, transform.position.x, transform.position.y));
        }

        [Server]
        public void Dig(DigAction digger)
        {
            currentDigger = digger;
            RpcSetDigger(digger.netIdentity);
            diggerID = digger.netIdentity;
            EventManager.Instance.TriggerEvent(new ServerDiggableDestroyData(projectile.Type, transform.position.x, transform.position.y, currentDigger));
            
            bool isBot = digger.GetType().Equals(typeof(MD.AI.BotDigAction));          
            
            if (!isBot)
            {
                TargetTriggerProjectileObtain(currentDigger.connectionToClient);
            }
            
            Destroy(gameObject);
        }

        [TargetRpc]
        private void TargetTriggerProjectileObtain(NetworkConnection target)
        {
            EventManager.Instance.TriggerEvent(new ProjectileObtainData(target.identity, (DiggableType) projectile.Type));
        }

        [ClientRpc]
        private void RpcSetDigger(NetworkIdentity id)
        {
            diggerID = id;
        }
    }
}