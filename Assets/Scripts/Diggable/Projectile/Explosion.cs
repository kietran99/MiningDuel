using Timer;
using UnityEngine;
using Mirror;

namespace MD.Diggable.Projectile
{
    [RequireComponent(typeof(Timer.Timer))]
    public class Explosion : NetworkBehaviour, Timer.ITickListener
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

        #endregion
        
        private ITimer timer = null;
        private bool isExploded = false;
        private bool isThrown = false;

        public override void OnStartServer()
        {
            base.OnStartServer();
            timer = GetComponent<ITimer>();
            timer.Activate();
        }
        
        [ServerCallback]
        public void OnTick(float timeStamp)
        {
            if (timeStamp == 2f)
            {
                spriteRenderer.color = Color.red;
            }

            if (timeStamp == 3f)
            {
                timer.Stop();
                if (!isExploded) Explode();
            }     
        }

        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isThrown) return;

            if (other.CompareTag(Constants.PLAYER_TAG))
            {
                ProjectileLauncher laucher =  transform.GetComponentInParent<ProjectileLauncher>();

                if (laucher)
                {
                    laucher.StopOnCollide();
                }

                Explode();
            }
        }

        [ServerCallback]
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(Constants.PLAYER_TAG)) isThrown = true;
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
            spriteRenderer.sprite = explodeSprite;
            Invoke(nameof(DestroyProjectile), .2f);
        }

        [ServerCallback]
        private void DestroyProjectile() => Destroy(projectileObject);
    }
}