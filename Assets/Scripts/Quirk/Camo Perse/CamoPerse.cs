using UnityEngine;
using Mirror;

namespace MD.Quirk
{
    [RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D))]
    public class CamoPerse : BaseQuirk
    {
        private readonly float GRID_OFFSET = .5f;

        [SerializeField]
        private Sprite[] sonarSprites = null;

        [SerializeField]
        private float armSeconds = 1f;

        [SerializeField]
        private float gemDropPercentage = 20;

        private NetworkIdentity planter = null, collidingTarget = null;

        // [Client]
        public override void Activate(NetworkIdentity userIdentity)
        {
            base.Activate(userIdentity); 
            planter = userIdentity;
            GetComponent<SpriteRenderer>().sprite = sonarSprites.Random();
            System.Func<float, float> SnapPosition = val => Mathf.FloorToInt(val) + GRID_OFFSET;
            transform.position = new Vector3(SnapPosition(planter.transform.position.x), SnapPosition(planter.transform.position.y), 0f);
            Invoke(nameof(StartArming), armSeconds);
            gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<DigInvokeData>(RequestExplosion);
        }
        
        private void StartArming()
        {
            GetComponent<CircleCollider2D>().enabled = true;
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
            if (netIdentity.hasAuthority) netIdentity.RemoveClientAuthority();         
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
