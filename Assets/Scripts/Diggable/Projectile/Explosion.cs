using Timer;
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
                    TargetNotifyBombExplodeOnHand(GetComponent<ProjectileLauncher>().GetOwner().connectionToClient);
                }                
            }
        }
        
        [TargetRpc]
        //notify for target client to play animations
        private void TargetNotifyBombExplodeOnHand(NetworkConnection conn)
        {
            Debug.Log("bomb expldoded on hand");
            EventSystems.EventManager.Instance.TriggerEvent(new ThrowInvokeData());
        }

        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isThrown) return;

            if (!other.CompareTag(Constants.PLAYER_TAG)) return;
            
            ProjectileLauncher laucher =  transform.GetComponentInParent<ProjectileLauncher>();
            if (laucher)
            {
                if (other.gameObject == laucher.source && Time.time < laucher.sourceCollidableTime) return;
                laucher.StopOnCollide();
            }

            Explode();          
        }

        [ServerCallback]
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG)) return;

            if (other.GetComponent<MD.Character.Player>().netIdentity == GetComponent<ProjectileLauncher>().GetOwner())
            {
                isThrown = true;
            }
        }

        [ServerCallback]
        private void Explode()
        {
            isExploded = true;
            // Debug.Log("Explode");
            // var mySphere =  GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // mySphere.transform.localScale = new Vector3(2f*explosionRadius,2f*explosionRadius,1f);
            // mySphere.transform.position = transform.position;
            
            CheckForCollision();
            PlayExplosionEffect();
            EventSystems.EventManager.Instance.TriggerEvent(new ExplodeData());
        }

        [ServerCallback]
        private void CheckForCollision()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, explodeLayerMask);

            foreach (Collider2D collide in colliders)
            {
                if (!collide.CompareTag(Constants.PLAYER_TAG)) continue;
                    
                IExplodable target = collide.transform.GetComponent<IExplodable>();
                target?.ProcessExplosion(stats.GemDropPercentage, stats.StunTime, -1);
            }
        }
        
        [ServerCallback]
        private void PlayExplosionEffect()
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