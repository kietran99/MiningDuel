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

        public float SourceCollidableTime { get; private set; }

        public override void OnStartClient()
        {
            transform.parent = thrower.gameObject.transform;
            transform.localPosition = new Vector3(0, 1f, 0);
        }
        
        [Server]
        public void Launch(float power, float dirX, float dirY)
        {
            transform.parent = null;
            SourceCollidableTime = Time.time + 1.5f;
            RpcLauch(dirX, dirY, power);
        }

        [ClientRpc]
        private void RpcLauch(float dirX, float dirY, float power)
        {
            transform.parent = null;
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