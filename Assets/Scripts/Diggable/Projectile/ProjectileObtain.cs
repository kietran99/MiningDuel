using UnityEngine;

namespace MD.Diggable.Projectile
{
    public class ProjectileObtain : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer = null;

        private bool diggable = false;

        void Start()
        {
            EventSystems.EventManager.Instance.StartListening<DigControlData>(Dig);
        }

        private void Dig(DigControlData obj)
        {
            if (!diggable) return;

            EventSystems.EventManager.Instance.TriggerEvent(new ProjectilePickupData(spriteRenderer.sprite));
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG)) return;

            diggable = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG)) return;

            diggable = false;
        }
    }
}