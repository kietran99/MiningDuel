using MD.Character;
using UnityEngine;
using Mirror;

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

        private Player player  =null;
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
            EventSystems.EventManager.Instance.TriggerEvent(
                new DiggableSpawnData(GemValue.Value,transform.position.x,transform.position.y));
        }

        public override void OnStopClient()
        {
            //fire an event for sonar to update
            EventSystems.EventManager.Instance.TriggerEvent(
                new DiggableDestroyData(GemValue.Value, transform.position.x, transform.position.y));
            //for animations and UIs
            if (diggerID != null && diggerID == Player.netIdentity)
            EventSystems.EventManager.Instance.TriggerEvent(
                new GemDigSuccessData(GemValue.Value, transform.position.x, transform.position.y)
            );            
        }

        [Server]
        public void Dig(DigAction digger)
        {           
            this.currentDigger = digger;
            GemValue.DecreaseValue(currentDigger.Power);
            this.diggerID = digger.netIdentity;
            RpcSetDigger(digger.netIdentity);
            if (diggerID != null && diggerID.Equals(Player.netIdentity)) 
            {
                EventSystems.EventManager.Instance.TriggerEvent(new DigProgressData(GemValue.RemainingHit, GemValue.Value));
            }

            if (GemValue.RemainingHit > 0) return;
           
            EventSystems.EventManager.Instance.TriggerEvent(
                new ServerDiggableDestroyData(GemValue.Value, transform.position.x, transform.position.y, currentDigger));
            Destroy(gameObject);
        }

        [ClientRpc]
        private void RpcSetDigger(NetworkIdentity id)
        {
            this.diggerID = id;
        }
        
    }
}
