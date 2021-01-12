using UnityEngine;
using Mirror;
using MD.Character;

namespace MD.Diggable.Gem
{
    public class DropObtain : NetworkBehaviour
    {
        [SerializeField]
        private int value = 1;

        [SerializeField]
        private bool obtainable;

        [SerializeField]
        private float obtainWaitTime = 3f;

        public override void OnStartServer()
        {
            obtainable = false;
            Invoke("EnableObtain", obtainWaitTime);
        }

        [Server]
        private void EnableObtain()
        {
            obtainable = true;
        }

        [ServerCallback]
        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG) || !obtainable) return;

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
