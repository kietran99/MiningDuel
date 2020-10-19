using UnityEngine;

namespace MD.Diggable.Projectile
{
    public class ProjectileObtain : MonoBehaviour
    {
        [SerializeField]
        private ProjectileStats stats = null;

        private bool diggable = false;

        void Start()
        {
            EventSystems.EventManager.Instance.StartListening<DigInvokeData>(Dig);
        }

        private void Dig(DigInvokeData obj)
        {
            if (!diggable) return;

            EventSystems.EventManager.Instance.TriggerEvent(new ProjectileObtainData(stats));
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