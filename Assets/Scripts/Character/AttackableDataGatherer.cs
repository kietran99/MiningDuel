using Mirror;
using UnityEngine;

namespace MD.Character
{
    public class AttackableDataGatherer : MonoBehaviour
    {
        public System.Action<NetworkIdentity> OnDamagableEnter { get; set; }
        public System.Action<NetworkIdentity> OnDamagableExit { get; set; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var damagable = other.GetComponent<IDamagable>();

            if (damagable == null)
            {
                return;
            }

            OnDamagableEnter?.Invoke(other.GetComponent<NetworkIdentity>());
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var damagable = other.GetComponent<IDamagable>();

            if (damagable == null)
            {
                return;
            }

            OnDamagableExit?.Invoke(other.GetComponent<NetworkIdentity>());
        }
    }
}
