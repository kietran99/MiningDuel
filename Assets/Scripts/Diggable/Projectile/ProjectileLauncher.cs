using UnityEngine;

namespace MD.Diggable.Projectile
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ProjectileLauncher : MonoBehaviour
    {
        private bool shouldLaunch = false;
        private Rigidbody2D rigidBody;
        private Vector3 holdPos;
        private Transform player;

        private Vector2 throwDir;
        private float power;

        void Start()
        {
            player = Player.Instance.transform;
            rigidBody = GetComponent<Rigidbody2D>();
            holdPos = rigidBody.transform.localPosition;
        }

        private void Update()
        {
            if (shouldLaunch) return;

            rigidBody.transform.position = player.position + holdPos;
        }

        void FixedUpdate()
        {
            // if (!shouldLaunch) return;

            // rigidBody.AddForce(throwDir.normalized * power, ForceMode2D.Impulse);
            //rigidBody.velocity = throwDir.normalized * power;
        }

        public void BindThrowDirection(Vector2 throwDir)
        {
            this.throwDir = throwDir;
        }

        public void Throw(float power)
        {
            this.power = power;
            shouldLaunch = true;
            rigidBody.AddForce(throwDir.normalized * power, ForceMode2D.Impulse);
        }
    }
}