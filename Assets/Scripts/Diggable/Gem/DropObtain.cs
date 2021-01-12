using UnityEngine;
using Mirror;
using MD.Character;

namespace MD.Diggable.Gem
{
    public class DropObtain : NetworkBehaviour
    {
        [SerializeField]
        private int value = 1;

        public uint ThrowerID { get; set; }

        [ServerCallback]
        void OnTriggerEnter2D(Collider2D other)
        {
            if (!ThrowerID.Equals(other.GetComponent<NetworkIdentity>().netId)) return;

            if (!other.CompareTag(Constants.PLAYER_TAG)) return;

            if (other.GetComponent<MD.Character.ScoreManager>() != null)
            {
                EventSystems.EventManager.Instance.TriggerEvent(new DropObtainData(other.GetComponent<Player>().netId, value));
            }
            else
            {
                other.GetComponent<PlayerBot>().score += value;
            }

            Destroy(gameObject);
        }
    }
}
