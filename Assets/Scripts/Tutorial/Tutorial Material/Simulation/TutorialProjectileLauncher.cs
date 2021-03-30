using UnityEngine;

namespace MD.Tutorial
{
    public class TutorialProjectileLauncher : MonoBehaviour
    {
        public Transform Player { get; set; }
        public float SourceCollidableTime { get; private set; }

        private bool shouldFollowPlayer = true;

        private void Update()
        {
            if (Player == null)
            {
                return;
            }

            if (!shouldFollowPlayer)
            {
                return;
            }

            transform.position = new Vector3(Player.position.x, Player.position.y + 1f, transform.position.z);
        }

        public void Launch(float power, float dirX, float dirY)
        {
            Debug.Log("Launch");
            shouldFollowPlayer = false;
            // transform.parent = null;
            SourceCollidableTime = Time.time + 1.5f;
            GetComponent<Rigidbody2D>().AddForce(new Vector2(dirX, dirY).normalized * power, ForceMode2D.Impulse);
        }
    }
}
