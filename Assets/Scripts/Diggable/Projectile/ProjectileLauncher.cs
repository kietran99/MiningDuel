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

        void Start()
        {
            player = transform;
            rigidBody = GetComponent<Rigidbody2D>();
            holdPos = rigidBody.transform.localPosition;
        }

        private void Update()
        {
            if (shouldLaunch) return;

            rigidBody.transform.position = player.position + holdPos;
        }
        
        public void BindThrowDirection(Vector2 throwDir) => this.throwDir = throwDir;

        public void Launch(float power)
        {
            shouldLaunch = true;
            rigidBody.AddForce(throwDir.normalized * power, ForceMode2D.Impulse);
        }

        public void StopOnCollide() => rigidBody.velocity = Vector2.zero;
    }
}