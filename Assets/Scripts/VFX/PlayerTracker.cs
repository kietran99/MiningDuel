using UnityEngine;
using MD.Character;

namespace MD.VisualEffects
{
    public class PlayerTracker : MonoBehaviour
    {
        private Transform player;

        private void Start()
        {
            if (!ServiceLocator.Resolve<Player>(out Player player))
            {
                return;
            }

            this.player = player.transform;
        }

        private void LateUpdate()
        {
            if (player == null)
            {
                return;
            }
            
            transform.position = GetFollowOffset(player.position);
        }

        private Vector3 GetFollowOffset(Vector3 playerPos) => new Vector3(playerPos.x, playerPos.y, transform.position.z);
    }
}

