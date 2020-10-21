using Timer;
using UnityEngine;

namespace MD.Diggable.Projectile
{
    [RequireComponent(typeof(Timer.Timer))]
    public class Explosion : MonoBehaviour, Timer.ITickListener
    {
        [SerializeField]
        private GameObject projectileObject = null;

        [SerializeField]
        private ProjectileStats stats = null;

        [SerializeField]
        private SpriteRenderer spriteRenderer = null;

        [SerializeField]
        private GameObject droppingGemPrefab = null;

        [SerializeField]
        private float maxExplosionForce = 50f;

        private ITimer timer = null;

        private bool canCollide = false;
        private bool isPlayerCollidedBoom = false;

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
                Explode();
                Destroy(gameObject);
            }     
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!canCollide || !other.CompareTag(Constants.PLAYER_TAG)) return;
            ExplodeWithPlayer(other.transform.position);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG)) return;
            canCollide = true;
        }

        private void ExplodeWithPlayer(Vector2 center)
        {
            IScoreManager scoreManager;
            GameObject droppingGem;
            bool exist = ServiceLocator.Resolve<IScoreManager>(out scoreManager);
            if (!exist) return;
            int numOfGem = Mathf.FloorToInt(scoreManager.GetCurrentScore()*stats.GemDropPercentage/100f);
            scoreManager.DecreaseScore(numOfGem);
            for (int i = 0; i< numOfGem; i++)
            {
                droppingGem = Instantiate(droppingGemPrefab, center, Quaternion.identity);
                droppingGem.GetComponent<Rigidbody2D>().AddForce(GetExplosionForce()*GetExplosionDirection());
            }
            EventSystems.EventManager.Instance.TriggerEvent(new ExplodeData());
            Destroy(projectileObject);
        }
        private void Explode()
        {
            Destroy(projectileObject);
        }

        private float GetExplosionForce()
        {
            return Random.Range(1f,maxExplosionForce);
        }
        private Vector2 GetExplosionDirection()
        {
            Vector2 randomDir = Vector2.zero;
            randomDir.x= Random.Range(-1f,1f);
            randomDir.y = Random.Range(-1f,1f);
            return randomDir.normalized;
        }
    }
}