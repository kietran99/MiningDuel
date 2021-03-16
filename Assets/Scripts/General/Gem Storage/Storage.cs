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
        private float checkTime;

        [SerializeField]
        private GameObject ProcessBar = null;

        [SerializeField]
        private Image ProcessBarImage = null;

        private NetworkIdentity ownerID;
        private bool isInside;

        public override void OnStartServer()
        {
            base.OnStartServer();
            checkTime = storeTime/TIMES_CHECK;
        }

        public void Initialize(NetworkIdentity ownerID)
        {
            this.ownerID = ownerID;
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
