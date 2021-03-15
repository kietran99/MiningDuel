using UnityEngine;
using Mirror;

namespace MD.Diggable.Projectile
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ProjectileLauncher : NetworkBehaviour
    {
        [SyncVar]
        private NetworkIdentity thrower;

        public NetworkIdentity Thrower { get => thrower; }

        // public float SourceCollidableTime { get; private set; }
        [SerializeField]
        private bool beingHeld;
        public bool BeingHeld => beingHeld;

        new Rigidbody2D rigidbody;
        private Explosion explosion;


        public override void OnStartClient()
        {
            // thrower.GetComponent<MD.Character.DigAction>().bombDug = true;
            beingHeld = true;
            // transform.parent = thrower.gameObject.transform;
            rigidbody = GetComponent<Rigidbody2D>();
            rigidbody.simulated = false;
            rigidbody.position = thrower.transform.position  + Vector3.up;
            explosion = GetComponent<Explosion>();
        }
        
        [Server]
        public void Launch(float power, float dirX, float dirY)
        {
            beingHeld = false;
            rigidbody.simulated = true;
            // transform.parent = null;
            // SourceCollidableTime = Time.time + 1.5f;
            if (explosion) explosion.NotifyThrow();
            RpcLauch(dirX, dirY, power);
        }

        private void LateUpdate()
        {
            if (beingHeld)
            {
                transform.position = thrower.transform.position  + Vector3.up;
            }
        }

        [ClientRpc]
        private void RpcLauch(float dirX, float dirY, float power)
        {
            beingHeld = false;
            // transform.parent = null;
            rigidbody.simulated = true;
            GetComponent<Rigidbody2D>().AddForce(new Vector2(dirX, dirY).normalized * power, ForceMode2D.Impulse);
        }
        
        [Server]
        public void SetThrower(NetworkIdentity thrower) => this.thrower = thrower;

        [Server]
        public void StopOnCollide() => RpcStopOnCollide();
        
        [ClientRpc]
        public void RpcStopOnCollide() => GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}