using UnityEngine;
using Mirror;

namespace MD.Quirk
{
    [RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D))]
    public class QuirkObtain : NetworkBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private BaseQuirk containingQuirk = null;
        private QuirkPouch quirkPouch;

        // Quirk Obtain is disabled by default -> Use RpcEnable to enable it & assign all its neccessary fields
        [ClientRpc]
        public void RpcEnable(GameObject quirkGO)
        {
            containingQuirk = quirkGO.GetComponent<BaseQuirk>();

            if (containingQuirk == null)
            {
                Debug.LogError("Not a Base Quirk GameObject");
                return;
            }

            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = containingQuirk.ObtainSprite;

            gameObject.SetActive(true);
        }

        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D other)
        {            
            if (!other.CompareTag(Constants.PLAYER_TAG))
            {
                return;
            }

            RpcPouchInsert(other.GetComponent<NetworkIdentity>());    
        }

        [ClientRpc]
        private void RpcPouchInsert(NetworkIdentity collidingPlayer)
        {
            quirkPouch = collidingPlayer.GetComponent<QuirkPouch>();

            if (quirkPouch == null)
            {
                Debug.Log("No Quirk Pouch is found on Player");
                return;
            }

            containingQuirk.transform.SetParent(quirkPouch.transform);
            quirkPouch.Insert(containingQuirk);
            NetworkServer.Destroy(gameObject);
        }
    }
}
