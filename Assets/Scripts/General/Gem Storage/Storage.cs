using System.Collections;
using UnityEngine;
using Mirror;
using MD.Character;
using UnityEngine.UI;

namespace MD.Diggable.Core
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Storage : NetworkBehaviour
    {
        private const float TIMES_CHECK = 20;

        [SerializeField]
        private float storeTime = 2f;


        [SerializeField]
        private GameObject ProcessBar = null;

        [SerializeField]
        private Image ProcessBarImage = null;

        [SerializeField]
        private SpriteRenderer crown = null;

        [SerializeField]
        private PlayerColorPicker playerColorPicker = null;

        private float checkTime;
        private NetworkIdentity ownerID;
        private bool isInside;
        
        [SyncVar]
        private Color flagColor;

        public override void OnStartServer()
        {
            checkTime = storeTime / TIMES_CHECK;
        }

        public void Initialize(NetworkIdentity ownerID, Color flagColor)
        {
            this.ownerID = ownerID;
            this.flagColor = flagColor;

        }

        public override void OnStartClient()
        {
            crown.sprite = playerColorPicker.GetCrownSprite(flagColor);
        }

        [ServerCallback]
        void OnTriggerEnter2D(Collider2D collide)
        {
            if (!collide.CompareTag(Constants.PLAYER_TAG)) 
            {
                return;
            }

            Player player = collide.gameObject.GetComponent<Player>();

            if (player == null || player.netIdentity != ownerID) 
            {
                return;
            }

            isInside = true;
            TargetShowProcessBar(ownerID.connectionToClient);
            StartCoroutine(nameof(StoringScore));
        }

        [ServerCallback]
        void OnTriggerExit2D(Collider2D collide)
        {
            if (!collide.CompareTag(Constants.PLAYER_TAG)) 
            {
                return;
            }

            Player player = collide.gameObject.GetComponent<Player>();
            if (player == null || player.netIdentity != ownerID) 
            {
                return;
            }

            TargetHideProcessBar(ownerID.connectionToClient);
            isInside = false;
        }

        IEnumerator StoringScore()
        {
            var waitTime = new WaitForSeconds(checkTime);
            for (int i = 1; i <= TIMES_CHECK; i++)
            {
                yield return waitTime;
                if (!isInside) yield break;
                //play animation in rpc
                TargetShowProcess(ownerID.connectionToClient, (float)i /TIMES_CHECK);
            }
            //storing finished, fire an event
            EventSystems.EventManager.Instance.TriggerEvent(new StoreFinishedData(ownerID));
        }

        [TargetRpc]
        private void TargetShowProcess(NetworkConnection conn, float amount)
        {
            if (!ProcessBar.activeInHierarchy) ProcessBar.SetActive(true);
            ProcessBarImage.fillAmount = amount;
        }

        [TargetRpc]
        private void TargetHideProcessBar(NetworkConnection conn)
        {
            ProcessBar.SetActive(false);
        }

        [TargetRpc]
        private void TargetShowProcessBar(NetworkConnection conn)
        {
            ProcessBarImage.fillAmount = 0;
            ProcessBar.SetActive(true);
        }
    }
}
