using MD.Character;
using UnityEngine;
using Mirror;
using EventSystems;

namespace MD.Diggable.Gem
{
    [RequireComponent (typeof(CircleCollider2D))]
    [RequireComponent(typeof(GemValue))]
    public class GemObtain : NetworkBehaviour, ICanDig
    {
        [SyncVar]
        private NetworkIdentity diggerID;
        private DigAction currentDigger;

        private MapManager mapMangerServer;

        private GemValue gemValue = null;

        private Player player = null;
        private Player Player
        {
            get
            {
                if (player != null) return player;
                ServiceLocator.Resolve<Player>(out player);
                return player;
            }
        }
        private GemValue GemValue
        {
            get
            {
                if (gemValue != null) return gemValue;
                return gemValue = GetComponent<GemValue>();
            }
        }  
        public override void OnStartClient()
        {
            base.OnStartClient();
            EventManager.Instance.TriggerEvent(new DiggableSpawnData(GemValue.Value, transform.position.x, transform.position.y));
        }

        public override void OnStopClient()
        {
            //fire an event for sonar to update
            EventManager.Instance.TriggerEvent(new DiggableDestroyData(GemValue.Value, transform.position.x, transform.position.y));                   
        }

        [Server]
        public void Dig(DigAction digger)
        {           
            currentDigger = digger;
            GemValue.DecreaseValue(digger.Power, out bool obtainable);
            RpcSetDigger(digger.netIdentity);
            diggerID = digger.netIdentity; 

            bool isBot = digger.GetType().Equals(typeof(BotDigAction));          
            
            if (!isBot)
            {
                TargetTriggerDigProgressData(digger.connectionToClient, GemValue.RemainingHit, GemValue.Value);
            }

            if (!obtainable) return;
           
            if (!isBot)
            {
                TargetTriggerGemDigSuccessData(digger.connectionToClient, GemValue.Value, transform.position.x, transform.position.y); 
            }

            EventManager.Instance.TriggerEvent(
                new ServerDiggableDestroyData(GemValue.Value, transform.position.x, transform.position.y, currentDigger));
                
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
            EventManager.Instance.TriggerEvent(new GemDigSuccessData(gemVal, x, y));
        }

        [ClientRpc]
        private void RpcSetDigger(NetworkIdentity id)
        {
            this.diggerID = id;
        }
        
    }
}
