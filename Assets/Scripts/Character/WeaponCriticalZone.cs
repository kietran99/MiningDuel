using UnityEngine;

namespace MD.Character
{
    public class WeaponCriticalZone : MonoBehaviour
    {
        public System.Action<Collider2D> OnCollide { get; set; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<CriticalPoint>(out var critPoint))
            {
                return;
            }

            OnCollide?.Invoke(critPoint.DamagableCollider);
        }
    }
}
