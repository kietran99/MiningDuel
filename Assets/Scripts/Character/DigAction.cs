using MD.Diggable.Projectile;
using UnityEngine;

namespace MD.Character
{
    [RequireComponent(typeof(ThrowAction))]
    public class DigAction : MonoBehaviour
    {
        [SerializeField]
        private int power = 1;
        

        [SerializeField]
        private GameObject bombPrefab = null;

        private ThrowAction throwAction;

        public int Power { get => power; }

        private void Start()
        {
            throwAction = GetComponent<ThrowAction>();
            EventSystems.EventManager.Instance.StartListening<ProjectileObtainData>(BindAndHoldProjectile);
        }

        private void OnDestroy()
        {
            EventSystems.EventManager.Instance.StopListening<ProjectileObtainData>(BindAndHoldProjectile);
        }

        private void BindAndHoldProjectile(ProjectileObtainData data)
        {
            throwAction.BindProjectile(Instantiate(bombPrefab, gameObject.transform));
        }
    }
}