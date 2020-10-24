using Timer;
using UnityEngine;

namespace MD.Diggable.Projectile
{
    [RequireComponent(typeof(Timer.Timer))]
    public class Explosion : MonoBehaviour, Timer.ITickListener
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
        private LayerMask explodeLayerMask;

        #endregion
        
        private ITimer timer = null;
        private bool isExploded = false;
        private bool isThrown = false;

        void Start()
        {
            timer = GetComponent<ITimer>();
            timer.Activate();
        }
        
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

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(Constants.PLAYER_TAG))  isThrown = true;
        }

        private void Explode()
        {
            isExploded = true;
            // Debug.Log("Explode");
            // var mySphere =  GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // mySphere.transform.localScale = new Vector3(2f*explosionRadius,2f*explosionRadius,1f);
            // mySphere.transform.position = transform.position;

            //check collision
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,explosionRadius, explodeLayerMask);
            IExplodable target;
            foreach (Collider2D collide in colliders)
            {
                if (collide.CompareTag(Constants.PLAYER_TAG))
                {
                    target = collide.transform.GetComponent<IExplodable>();
                    if (target != null) target.ProcessExplosion(stats.GemDropPercentage,stats.StunTime, -1);
                }
            }

            //effects
            spriteRenderer.sprite = explodeSprite;
            Invoke(nameof(DestroyProjectile), .2f);
            EventSystems.EventManager.Instance.TriggerEvent(new ExplodeData());
        }

        private void DestroyProjectile() => Destroy(projectileObject);

    }
}