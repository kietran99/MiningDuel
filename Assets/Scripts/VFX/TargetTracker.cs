using UnityEngine;
using MD.Character;

namespace MD.VisualEffects
{
    public class TargetTracker : MonoBehaviour
    {
        private Transform playerTransform;

        void Start()
        {
            if (!ServiceLocator.Resolve<Player>(out Player player))
            {
                return;
            }

            playerTransform = player.transform;
        }

        void Update()
        {
            if (playerTransform == null)
            {
                return;
            }

            transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
        }
    }
}
