using Timer;
using UnityEngine;
using Mirror;
using MD.UI;

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
        private Sprite almostExplodeSprite = null;

        [SerializeField]
        private Sprite explodeSprite = null;

        [SerializeField]
        private ParticleSystem sparkEffect = null;

        [SerializeField]
        private float almostExplosionSparkEmissionMultiplier = 3f;

        [SerializeField]
        private LayerMask explodeLayerMask = 1;

        [SerializeField]
        private bool isThrown = false;
        #endregion
        
        private ITimer timer = null;
        private bool isExploded = false;
        private bool shouldExplode = true;
        private ProjectileLauncher launcher;
        private bool canCollideWithThrower = false;
        
        public override void OnStartServer()
        {
            launcher = GetComponent<ProjectileLauncher>();
            timer = GetComponent<ITimer>();
            timer.Activate();
        }
        public void StopExplosion()
        {
            shouldExplode = false;
            Debug.Log("Bomb should not expolde");
        }
        
        [ServerCallback]
        public void OnTick(float timeStamp)
        {
            if (timeStamp == 2f)
            {
                RpcPlayAlmostExplodeEffects();
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
            if(!shouldExplode)
            {
                shouldExplode = true;
                return;
            }
            // if (!other.CompareTag(Constants.PLAYER_TAG)) return;
            if (other.GetComponent<IExplodable>() == null) return;
            if (launcher)
            {
                if (other.gameObject == launcher.Thrower.gameObject && !canCollideWithThrower) return;
                launcher.StopOnCollide();
            }

            Explode();          
        }

        // [ServerCallback]
        // private void OnTriggerExit2D(Collider2D other)
        // {
        //     if (!other.CompareTag(Constants.PLAYER_TAG) || isThrown) return;

        //     if (other.GetComponent<MD.Character.ThrowAction>().netIdentity == GetComponent<ProjectileLauncher>().Thrower)
        //     {
        //         isThrown = true;
        //     }
        // }

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
                // if (!collide.CompareTag(Constants.PLAYER_TAG)) continue;  

                IExplodable target = collide.transform.GetComponent<IExplodable>();
                if (target == null) continue;
                var thrower = GetComponent<ProjectileLauncher>().Thrower;
                target?.HandleExplosion(thrower.transform, thrower.netId, stats.GemDropPercentage);
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
        private void RpcPlayAlmostExplodeEffects() 
        {
            spriteRenderer.sprite = almostExplodeSprite;
            var emissionModule = sparkEffect.emission;
            emissionModule.rateOverTimeMultiplier *= almostExplosionSparkEmissionMultiplier;
        }

        [ClientRpc]
        private void RpcPlayExplosionEffect() => spriteRenderer.sprite = explodeSprite;

        [Server]
        public void NotifyThrow()
        {
            isThrown = true;
            Invoke(nameof(SetCanCollideWithThrower),1f);
        }

        [Server]
        private void SetCanCollideWithThrower() => canCollideWithThrower = true;
    }
}