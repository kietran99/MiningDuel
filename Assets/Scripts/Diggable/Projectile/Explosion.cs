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
        private Sprite explodeSprite = null;

        private ITimer timer = null;

        private bool canCollide = false;

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
            }     
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!canCollide || !other.CompareTag(Constants.PLAYER_TAG)) return;

            Explode();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG)) return;

            canCollide = true;
        }

        private void Explode()
        {
            spriteRenderer.sprite = explodeSprite;
            Invoke(nameof(Destroy), .2f);
            EventSystems.EventManager.Instance.TriggerEvent(new ExplodeData());
        }

        private void Destroy()
        {
            Destroy(projectileObject);
        }
    }
}