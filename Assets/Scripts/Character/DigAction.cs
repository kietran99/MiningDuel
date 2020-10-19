using MD.Diggable.Projectile;
using UnityEngine;

namespace MD.Character
{
    public class DigAction : MonoBehaviour
    {
        [SerializeField]
        private int power = 1;

        [SerializeField]
        private GameObject projectile = null;

        public int Power { get => power; }

        private void Start()
        {
            EventSystems.EventManager.Instance.StartListening<ProjectilePickupData>(ShowProjectileSprite);
        }

        private void OnDestroy()
        {
            EventSystems.EventManager.Instance.StopListening<ProjectilePickupData>(ShowProjectileSprite);
        }

        private void ShowProjectileSprite(ProjectilePickupData data)
        {
            projectile.SetActive(true);
            projectile.GetComponent<SpriteRenderer>().sprite = data.sprite;
        }
    }
}