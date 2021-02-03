using MD.Character;
using UnityEngine;
using Mirror;
using EventSystems;

namespace MD.Diggable.Gem
{
    [RequireComponent (typeof(CircleCollider2D))]
    [RequireComponent(typeof(GemValue))]
    public class GemObtain : NetworkBehaviour, IDiggable
    {
        [SyncVar]
        private NetworkIdentity diggerID;
        private DigAction currentDigger;
        private GemValue gemValue;
        
        public override void OnStartClient()
        {
            gemValue = GetComponent<GemValue>();
            // EventManager.Instance.TriggerEvent(
                // new DiggableSpawnData((int) transform.position.x, (int) transform.position.y), gemValue.Value as DiggableType);
        }

        public override void OnStopClient()
        {
            EventManager.Instance.TriggerEvent(new DiggableDestroyData(gemValue.Value, transform.position.x, transform.position.y));                   
        }

        [Server]
        public void Dig(DigAction digger)
        {           
            currentDigger = digger;
            gemValue.DecreaseValue(digger.Power, out bool obtainable);
            RpcSetDigger(digger.netIdentity);
            diggerID = digger.netIdentity; 

            bool isBot = digger.GetType().Equals(typeof(MD.AI.BotDigAction));          
            
            if (!isBot)
            {
                TargetTriggerDigProgressData(digger.connectionToClient, gemValue.RemainingHit, gemValue.Value);
            }

            if (obtainable) 
            {
                Obtain(isBot);
            }
        }

        [Server]
        private void Obtain(bool isBot)
        {
            if (!isBot)
            {
                TargetTriggerGemDigSuccessData(currentDigger.connectionToClient, gemValue.Value, transform.position.x, transform.position.y); 
            }

            EventManager.Instance.TriggerEvent(
                new ServerDiggableDestroyData(gemValue.Value, transform.position.x, transform.position.y, currentDigger));
                
            Destroy(gameObject);
        }

        [TargetRpc]
        private void TargetTriggerDigProgressData(NetworkConnection target, int remainingHit, int initialValue)
        {
            EventManager.Instance.TriggerEvent(new DigProgressData(remainingHit, initialValue));
        }

        [TargetRpc]
        private void TargetTriggerGemDigSuccessData(NetworkConnection target, int gemVal, float x, float y)
        {
            EventManager.Instance.TriggerEvent(new GemDigSuccessData(diggerID.netId, x, y, gemVal));
        }

        [ClientRpc]
        private void RpcSetDigger(NetworkIdentity id)
        {
            this.diggerID = id;
        }      
    }
}
