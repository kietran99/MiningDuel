﻿using Timer;
using UnityEngine;
using Mirror;

namespace MD.Diggable.Projectile
{
    [RequireComponent(typeof(Timer.Timer))]
    public class Explosion : NetworkBehaviour, ITickListener
    {
        #region  SERIALIZE FIELDS
        [SerializeField]
        private float explosionRadius = 1.5f;

        [SerializeField]
        private GameObject projectileObject = null;

        [SerializeField]
        private ProjectileStats stats = null;

        [SerializeField]
        private SpriteRenderer spriteRenderer = null;

        [SerializeField]
        private Sprite explodeSprite = null;

        [SerializeField]
        private LayerMask explodeLayerMask = 1;

        [SerializeField]
        private bool isThrown = false;
        #endregion
        
        private ITimer timer = null;
        private bool isExploded = false;

        public override void OnStartServer()
        {
            timer = GetComponent<ITimer>();
            timer.Activate();
        }
        
        [ServerCallback]
        public void OnTick(float timeStamp)
        {
            if (timeStamp == 2f)
            {
                RpcChangeSpriteColor();
            }

            if (timeStamp == 3f)
            {
                timer.Stop();

                if (isExploded) return;

                Explode();
                if (!isThrown)
                {
                    var botAnim = GetComponent<ProjectileLauncher>().Thrower.GetComponent<BotAnimator>();
                    if (botAnim)
                        botAnim.RevertToIdleState();
                    else
                        TargetNotifyBombExplodeOnHand(GetComponent<ProjectileLauncher>().Thrower.connectionToClient);
                }                
            }
        }
        
        [TargetRpc]
        //notify for target client to play animations
        private void TargetNotifyBombExplodeOnHand(NetworkConnection conn)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new ThrowInvokeData());
        }

        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isThrown) return;

            if (!other.CompareTag(Constants.PLAYER_TAG)) return;
            
            var launcher = GetComponent<ProjectileLauncher>();
            if (launcher)
            {
                if (other.gameObject == launcher.Thrower && Time.time < launcher.SourceCollidableTime) return;

                launcher.StopOnCollide();
            }

            Explode();          
        }

        [ServerCallback]
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG)) return;

            if (other.GetComponent<MD.Character.ThrowAction>().netIdentity == GetComponent<ProjectileLauncher>().Thrower)
            {
                isThrown = true;
            }
        }

        [ServerCallback]
        private void Explode()
        {
            isExploded = true;
            CheckForCollision();
            PlayExplodingEffect();
            EventSystems.EventManager.Instance.TriggerEvent(new ProjectileCollisionData());
        }

        [ServerCallback]
        private void CheckForCollision()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, explodeLayerMask);

            foreach (Collider2D collide in colliders)
            {
                if (!collide.CompareTag(Constants.PLAYER_TAG)) continue;  

                IExplodable target = collide.transform.GetComponent<IExplodable>();
                target?.HandleExplosion(GetComponent<ProjectileLauncher>().Thrower.netId, stats.GemDropPercentage, -1);
            }
        }
        
        [ServerCallback]
        private void PlayExplodingEffect()
        {
            RpcPlayExplosionEffect();
            Invoke(nameof(DestroyProjectile), .2f);
        }

        [ServerCallback]
        private void DestroyProjectile() => Destroy(projectileObject);

        [ClientRpc]
        private void RpcChangeSpriteColor() => spriteRenderer.color = Color.red;

        [ClientRpc]
        private void RpcPlayExplosionEffect() => spriteRenderer.sprite = explodeSprite;
    }
}