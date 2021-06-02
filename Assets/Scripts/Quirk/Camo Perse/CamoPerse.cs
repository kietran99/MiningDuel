using UnityEngine;
using Mirror;

namespace MD.Quirk
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CamoPerse : NetworkBehaviour
    {
        private readonly float GRID_OFFSET = .5f;

        [SerializeField]
        private CamoPerseAnimHandler animHandler = null;

        [SerializeField]
        private float gemDropPercentage = 20;

        private NetworkIdentity planter = null, collidingTarget = null;

        // [Client]
        // public override void SyncActivate(NetworkIdentity userIdentity)
        // {
        //     base.SyncActivate(userIdentity); 
        //     planter = userIdentity;
        //     System.Func<float, float> SnapPosition = val => Mathf.FloorToInt(val) + GRID_OFFSET;
        //     transform.position = new Vector3(SnapPosition(planter.transform.position.x), SnapPosition(planter.transform.position.y), 0f);
        //     gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<CamoPerseAnimEndData>(HandleAnimEndEvent);
        //     animHandler.PlayAnimation();
        // }
        
        [ClientRpc]
        public void RpcSetUp(NetworkIdentity userIdentity)
        {
            planter = userIdentity;
            System.Func<float, float> SnapPosition = val => Mathf.FloorToInt(val) + GRID_OFFSET;
            transform.position = new Vector3(SnapPosition(planter.transform.position.x), SnapPosition(planter.transform.position.y), 0f);
            gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<CamoPerseAnimEndData>(HandleAnimEndEvent);
            animHandler.PlayAnimation();
        }
        
        private void HandleAnimEndEvent(CamoPerseAnimEndData _)
        {
            GetComponent<CircleCollider2D>().enabled = true;
            GetComponent<EventSystems.EventConsumer>().StartListening<DigInvokeData>(RequestExplosion);
        }

        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG))
            {
                return;
            }

            var collidingIdentity = other.GetComponent<NetworkIdentity>();
            RpcBindCollidingTarget(collidingIdentity);   
            netIdentity.RemoveClientAuthority();         
            netIdentity.AssignClientAuthority(collidingIdentity.connectionToClient);
        }

        [ServerCallback]
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG))
            {
                return;
            }

            RpcBindCollidingTarget(null);
            netIdentity.RemoveClientAuthority();
        }      

        [ClientRpc]
        private void RpcBindCollidingTarget(NetworkIdentity collidingTarget)
        {
            this.collidingTarget = collidingTarget;
        }

        [Client]
        private void RequestExplosion(DigInvokeData _)
        {
            if (collidingTarget == null || !collidingTarget.hasAuthority)
            {
                return;
            }

            CmdExplodeIfPlayerInRange();
        }

        [Command]
        private void CmdExplodeIfPlayerInRange()
        {
            var explodeTarget = collidingTarget.GetComponent<Diggable.Projectile.IExplodable>();

            if (explodeTarget == null)
            {
                Debug.Log("Camo Perse: Unexplodable Digger");
                return;
            }
            
            explodeTarget?.HandleExplosion(planter.transform, planter.netId, gemDropPercentage);

            GetComponent<CircleCollider2D>().enabled = false;
            RpcBindCollidingTarget(null);
            NetworkServer.Destroy(gameObject);
        }
    }
}
