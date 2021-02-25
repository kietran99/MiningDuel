﻿using UnityEngine;
using Mirror;

namespace MD.Quirk
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class QuirkObtain : NetworkBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private BaseQuirk containingQuirk = null;
        private QuirkPouch quirkPouch;
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

        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D other)
        {            
            if (!other.CompareTag(Constants.PLAYER_TAG))
            {
                return;
            }

            var collidingIdentity = other.GetComponent<NetworkIdentity>();    
            RpcBindCollidingTarget(collidingIdentity);
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

        private void RequestObtain(DigInvokeData _)
        {
            if (collidingIdentity == null || !collidingIdentity.hasAuthority)
            {
                return;
            }

            CmdObtain();
            CmdAssignAuthority(containingQuirk.netIdentity, collidingIdentity);
        }

        [Command]
        private void CmdObtain() => RpcPouchInsert(collidingIdentity);

        [Command]
        private void CmdAssignAuthority(NetworkIdentity quirk, NetworkIdentity player) => quirk.AssignClientAuthority(player.connectionToClient);

        [ClientRpc]
        private void RpcPouchInsert(NetworkIdentity collidingPlayer)
        {
            quirkPouch = collidingPlayer.GetComponent<QuirkPouch>();

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
