using UnityEngine;
using Mirror;

namespace MD.Diggable.Projectile
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ProjectileLauncher : NetworkBehaviour
    {       
        private Vector3 holdPos;

        [SyncVar]
        private NetworkIdentity owner;
        public GameObject source = null;

        public float sourceCollidableTime  = 0f;

        public override void OnStartClient()
        {
            transform.parent = owner.gameObject.transform;
            //set bomb holding position
            transform.localPosition  =  new Vector3(0,1f,0);
        }
        

        [Server]
        public void Launch(float power, float dirX, float dirY)
        {
            source = owner.gameObject;
            transform.parent =null;
            //cant collide with Source for 1.5s second after lauch
            sourceCollidableTime = Time.time + 1.5f;
            RpcLauch(dirX, dirY, power);
        }

        [ClientRpc]
        private void RpcLauch(float dirX, float dirY, float power)
        {
            transform.parent = null;
            GetComponent<Rigidbody2D>().AddForce(new Vector2(dirX,dirY).normalized*power,ForceMode2D.Impulse);
        }
        
        [Server]
        public void SetOwner(NetworkIdentity owner)
        {
            this.owner = owner;
        }

        [Server]
        public void StopOnCollide(){
            RpcStopOnCollide();
        }
        
        [ClientRpc]
        public void RpcStopOnCollide()
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        public NetworkIdentity GetOwner() => owner;
    }
}