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

        public override void OnStartClient()
        {
            transform.parent = owner.gameObject.transform;
            //set bomb holding position
            transform.localPosition  =  new Vector3(0,1f,0);
        }
        

        [Server]
        public void Launch(float power, float dirX, float dirY)
        {
            RpcLauch(dirX, dirY, power);
        }

        [ClientRpc]
        private void RpcLauch(float dirX, float dirY, float power)
        {
            Debug.Log("call rpclauch on client throwDir dirx " + dirX +" diry "+ dirY + " power" + power);
            transform.parent = null;
            // transform.Translate(dir);
            GetComponent<Rigidbody2D>().AddForce(new Vector2(dirX,dirY).normalized*power,ForceMode2D.Impulse);
        }
        
        [Server]
        public void SetOwner(NetworkIdentity owner)
        {
            this.owner = owner;
        }
        [Server]
        public void StopOnCollide() => GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}