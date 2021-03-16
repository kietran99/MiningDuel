using UnityEngine;
using Mirror;

namespace MD.Quirk
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class QuirkObtain : NetworkBehaviour
    {
        [SerializeField]
        private QuirkMapper quirkMapper = null;

        private BaseQuirk containingQuirk;
        private QuirkType containingQuirkType;
        private NetworkIdentity collidingIdentity;

        // Quirk Obtain is disabled by default -> Use RpcInitialize to enable it & assign all its neccessary fields
        [ClientRpc]
        public void RpcInitialize(GameObject quirkGO)
        {
            containingQuirk = quirkGO.GetComponent<BaseQuirk>();

            if (containingQuirk == null)
            {
                Debug.LogError("Not a Base Quirk GameObject");
                return;
            }

            containingQuirk.transform.SetParent(transform);
            gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<DigInvokeData>(RequestObtain);

            gameObject.SetActive(true);
        }

        [ClientRpc]
        public void RpcInit(QuirkType type)
        {
            containingQuirkType = type;
            gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<DigInvokeData>(RequestObtain);
            gameObject.SetActive(true);
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
        private void RpcBindCollidingTarget(NetworkIdentity collidingIdentity) => this.collidingIdentity = collidingIdentity;

        [Client]
        private void RequestObtain(DigInvokeData _)
        {
            if (collidingIdentity == null || !collidingIdentity.hasAuthority)
            {
                return;
            }

            CmdRequestSpawnQuirk(containingQuirkType);
            CmdObtain();
        }

        [Command]
        private void CmdRequestSpawnQuirk(QuirkType type)
        {
            var obtainingQuirk = Instantiate(quirkMapper.Map(type));
            NetworkServer.Spawn(obtainingQuirk, collidingIdentity.connectionToClient);
            RpcBindContainingQuirk(obtainingQuirk.GetComponent<NetworkIdentity>());
        }

        [ClientRpc]
        private void RpcBindContainingQuirk(NetworkIdentity quirkIdentity) => containingQuirk = quirkIdentity.GetComponent<BaseQuirk>();

        [Command]
        private void CmdObtain() => RpcPouchInsert(collidingIdentity);

        [ClientRpc]
        private void RpcPouchInsert(NetworkIdentity collidingPlayer)
        {
            var quirkPouch = collidingPlayer.GetComponent<QuirkPouch>();

            if (quirkPouch == null)
            {
                Debug.Log("No Quirk Pouch is found on Player");
                return;
            }

            var success = quirkPouch.TryInsert(containingQuirk);

            if (success)
            {
                GetComponent<CircleCollider2D>().enabled = false;
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
