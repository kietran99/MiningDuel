﻿using UnityEngine;
using Mirror;
using MD.Character;

namespace MD.Diggable.Projectile
{
    [RequireComponent(typeof(DiggableProjectile))]
    public class ProjectileObtain : NetworkBehaviour, ICanDig
    {
        // private bool diggable = false;

        private DigAction currentDigger = null;

        private NetworkIdentity diggerID;
        private Player player =null;
        private Player Player
        {
            get
            {
                if (player != null) return player;
                ServiceLocator.Resolve<Player>(out player);
                return player;
            }
        }
        private DiggableProjectile projectile = null;
        private DiggableProjectile Projectile
        {
            get
            {
                if (projectile != null) return projectile;
                return projectile = GetComponent<DiggableProjectile>();
            }
        }  
        public override void OnStartClient()
        {
            EventSystems.EventManager.Instance.TriggerEvent(
                new DiggableSpawnData(Projectile.DiggbleType(),transform.position.x,transform.position.y));
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            //fire an event for sonar to update
            EventSystems.EventManager.Instance.TriggerEvent(
                new DiggableDestroyData(Projectile.DiggbleType(),transform.position.x,transform.position.y));
            //for animations and UI
            if (diggerID != null && diggerID == Player.netIdentity)
            {
            EventSystems.EventManager.Instance.TriggerEvent(
                new ProjectileObtainData(Projectile.GetStats(),transform.position.x,transform.position.y)
            );
            }
        }

        [Server]
        public void Dig(DigAction digger)
        {
            currentDigger = digger;
            RpcSetDigger(digger.netIdentity);
            this.diggerID =digger.netIdentity;
            EventSystems.EventManager.Instance.TriggerEvent(
                new ServerDiggableDestroyData(Projectile.DiggbleType(),transform.position.x,transform.position.y, currentDigger));
            Destroy(gameObject);
        }

        [ClientRpc]
        private void RpcSetDigger(NetworkIdentity id)
        {
            this.diggerID = id;
        }
    }
}