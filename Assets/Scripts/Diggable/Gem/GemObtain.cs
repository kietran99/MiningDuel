using MD.Character;
using UnityEngine;
using Mirror;

namespace MD.Diggable.Gem
{
    [RequireComponent (typeof(CircleCollider2D))]
    [RequireComponent(typeof(GemValue))]
    public class GemObtain : NetworkBehaviour, ICanDig
    {
        private DigAction currentDigger;

        private MapManager mapMangerServer;

        private GemValue gemValue = null;
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
            EventSystems.EventManager.Instance.TriggerEvent(
                new GemDigSuccessData(GemValue.Value, transform.position.x, transform.position.y)
            );            
        }

        [Server]
        public void Dig(DigAction digger)
        {           
            this.currentDigger = digger;
            GemValue.DecreaseValue(currentDigger.Power);

            if (GemValue.RemainingHit > 0) return;

            EventSystems.EventManager.Instance.TriggerEvent(
                new ServerDiggableDestroyData(GemValue.Value, transform.position.x, transform.position.y, currentDigger));
            Destroy(gameObject);
        }
        
    }
}
