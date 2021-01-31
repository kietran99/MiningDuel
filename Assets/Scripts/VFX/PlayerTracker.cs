using UnityEngine;
using MD.Character;

namespace MD.VisualEffects
{
    public class PlayerTracker : MonoBehaviour
    {
        protected Transform playerTransform;

        protected virtual void Start()
        {
            if (!ServiceLocator.Resolve<Player>(out Player player))
            {
                return;
            }

            playerTransform = player.transform;
        }

        private void FixedUpdate()
        {
            if (playerTransform == null)
            {
                return;
            }
            
            transform.position = GetFollowOffset(playerTransform.position);
        }

        protected virtual Vector3 GetFollowOffset(Vector3 playerPos) => new Vector3(playerPos.x, playerPos.y, transform.position.z);
    }
}

