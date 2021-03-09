using UnityEngine;

namespace MD.Tutorial
{
    public class TutorialExplosion : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer = null;

        [SerializeField]
        private Sprite explodeSprite = null;

        private Transform player;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag(Constants.PLAYER_TAG).transform;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var explodable = other.GetComponent<Diggable.Projectile.IExplodable>();
            if (explodable == null)
            {
                return;
            }

            explodable.HandleExplosion(player, 0, 0);
            spriteRenderer.sprite = explodeSprite;
            Invoke(nameof(Disable), .1f);
        }

        private void Disable() => gameObject.SetActive(false);
    }
}
