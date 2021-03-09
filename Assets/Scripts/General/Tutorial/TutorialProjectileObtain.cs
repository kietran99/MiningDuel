using UnityEngine;

namespace MD.Tutorial
{
    public class TutorialProjectileObtain : TutorialDiggableObtain
    {
        [SerializeField]
        private TutorialProjectileLauncher exposedBomb = null;

        private Collider2D collidingPlayer;

        protected override void TriggerObtainEvent()
        {
            exposedBomb.gameObject.SetActive(true);
            exposedBomb.Player = collidingPlayer.transform;
            collidingPlayer.GetComponent<TutorialThrowAction>().SetLauncher(exposedBomb);
            EventSystems.EventManager.Instance.TriggerEvent(new Diggable.Projectile.ProjectileObtainData(null, DiggableType.NORMAL_BOMB));
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other.CompareTag(Constants.PLAYER_TAG)) collidingPlayer = other;
        }
    }
}
