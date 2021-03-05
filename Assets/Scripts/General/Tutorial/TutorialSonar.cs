using UnityEngine;

namespace MD.Tutorial
{
    public class TutorialSonar : MonoBehaviour
    {
        private Transform player;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag(Constants.PLAYER_TAG).transform;
        }

        private void Update()
        {
            if (player == null)
            {
                Debug.LogWarning("No Game Object with Player tag was found");
                return;
            }

            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        }
    }
}
