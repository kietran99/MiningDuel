using UnityEngine;
using Mirror;

namespace MD.Quirk
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CamoPerse : NetworkBehaviour
    {
        [SerializeField]
        private CamoPerseAnimHandler animHandler = null;

        [SerializeField]
        private float gemDropPercentage = 20;

        [SerializeField]
        private int power = 15;

        private NetworkIdentity planter = null, collidingTarget = null;
        
        [ClientRpc]
        public void RpcSetUp(NetworkIdentity userIdentity)
        {
            planter = userIdentity;
            float gridOffset = .5f;
            System.Func<float, float> SnapPosition = val => Mathf.FloorToInt(val) + gridOffset;
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
            if (!collidingTarget.TryGetComponent<Diggable.Projectile.IExplodable>(out var explodeTarget))
            {
                return;
            }
            
            explodeTarget.HandleExplosion(planter.transform, planter.netId, gemDropPercentage);
            collidingTarget.GetComponent<Character.IDamagable>()?.TakeDamage(planter, power, false);
            GetComponent<CircleCollider2D>().enabled = false;
            
            NetworkServer.Destroy(gameObject);
        }

        private void OnDestroy()
        {
            EventSystems.EventManager.Instance.TriggerEvent(new VisualEffects.ExplosionEffectRequestData(transform.position));
        }
    }
}
