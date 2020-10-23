using Timer;
using UnityEngine;

namespace MD.Diggable.Projectile
{
    [RequireComponent(typeof(Timer.Timer))]
    public class Explosion : MonoBehaviour, Timer.ITickListener
    {
        [SerializeField]
        private float explosionRadius = 5f;

        [SerializeField]
        private GameObject projectileObject = null;

        [SerializeField]
        private ProjectileStats stats = null;

        [SerializeField]
        private SpriteRenderer spriteRenderer = null;

        [SerializeField]
        private Sprite explodeSprite = null;

        [SerializeField]
        private GameObject droppingGemPrefab = null;

        [SerializeField]
        private float maxExplosionForce = 250f;

        private ITimer timer = null;

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
                // if (!isThrown)
                // {
                //     ExplodeWithPlayer(transform.position);
                // }
                // else
                // {
                Explode();
                // }
            }     
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isThrown) return;
            // ExplodeWithPlayer(other.transform.position);
            if (other.CompareTag(Constants.PLAYER_TAG)) Explode();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(Constants.PLAYER_TAG))  isThrown = true;
        }

        // private void ExplodeWithPlayer(Vector2 center)
        // {
        //     GameObject droppingGem;
        //     if (!ServiceLocator.Resolve<IScoreManager>(out IScoreManager scoreManager)) return;
            
        //     int numOfGem = Mathf.FloorToInt(scoreManager.GetCurrentScore()*stats.GemDropPercentage/100f);
        //     scoreManager.DecreaseScore(numOfGem);
        //     for (int i = 0; i < numOfGem; i++)
        //     {
        //         droppingGem = Instantiate(droppingGemPrefab, center, Quaternion.identity);
        //         droppingGem.GetComponent<Rigidbody2D>().AddForce(GetExplosionForce() * GetExplosionDirection());
        //     }

        //     Explode();
        // }
        private void Explode()
        {
            //check collision
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,explosionRadius);
            foreach (Collider2D collide in colliders)
            {
                if (collide.CompareTag(Constants.PLAYER_TAG))
                {
                    Debug.Log(collide.transform.name);
                }
            }

            //effects
            spriteRenderer.sprite = explodeSprite;
            Invoke(nameof(DestroyProjectile), .2f);
            EventSystems.EventManager.Instance.TriggerEvent(new ExplodeData());
        }        

        private void DestroyProjectile() => Destroy(projectileObject);

        private float GetExplosionForce()
        {
            return Random.Range(100f, maxExplosionForce);
        }
        private Vector2 GetExplosionDirection()
        {
            Vector2 randomDir = Vector2.zero;
            randomDir.x = Random.Range(-1f, 1f);
            randomDir.y = Random.Range(-1f, 1f);
            return randomDir.normalized;
        }
    }
}