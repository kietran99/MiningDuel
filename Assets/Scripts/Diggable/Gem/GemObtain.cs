using MD.Character;
using UnityEngine;
using Mirror;

namespace MD.Diggable.Gem
{
    [RequireComponent (typeof(CircleCollider2D))]
    [RequireComponent(typeof(GemValue))]
    public class GemObtain : MonoBehaviour, ICanDig
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
              
        [Client]
        void Start()
        {
            EventSystems.EventManager.Instance.TriggerEvent(
                new DiggableSpawnData(GemValue.Value,transform.position.x,transform.position.y));
        }

        [Client]
        void OnDestroy()
        {
            //fire an event for sonar to update
            EventSystems.EventManager.Instance.TriggerEvent(
                new DiggableDestroyData(GemValue.Value, transform.position.x, transform.position.y));            
        }

        [Server]
        public void Dig(DigAction digger)
        {           
            this.currentDigger = digger;
            GemValue.DecreaseValue(currentDigger.Power);

            if (GemValue.RemainingHit > 0) return;

            EventSystems.EventManager.Instance.TriggerEvent(
                new GemDigSuccessData(transform.position.x, transform.position.y, GemValue.Value, currentDigger));

            Destroy(gameObject);
        }
        
    }
}
