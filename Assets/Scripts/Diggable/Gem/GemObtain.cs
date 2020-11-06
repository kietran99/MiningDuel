using MD.Character;
using UnityEngine;
using Mirror;
namespace MD.Diggable.Gem
{
    [RequireComponent (typeof(CircleCollider2D))]
    [RequireComponent(typeof(GemValue))]
    public class GemObtain : MonoBehaviour
    {
        private GemValue gemValue;
        private DigAction currentDigger;

        private MapManager mapMangerServer;
        // private bool diggable;
        [Server]
        void Start()
        {
            gemValue = GetComponent<GemValue>();
            // diggable = false;
            // EventSystems.EventManager.Instance.StartListening<DigInvokeData>(Dig);
        }

        [Server]
        public void Dig(DigAction digger)
        {           
            // if (!diggable) return;
            this.currentDigger = digger;
            gemValue.DecreaseValue(currentDigger.Power);

            if (gemValue.RemainingHit > 0) return;

            EventSystems.EventManager.Instance.TriggerEvent(
                new GemDigSuccessData(transform.position.x, transform.position.y, gemValue.Value, currentDigger));

            Destroy(gameObject);
        }
        
        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (!other.CompareTag(Constants.PLAYER_TAG)) return;
            
        //     diggable = true;
        //     digger = other.GetComponent<DigAction>();
        // }

        // private void OnTriggerExit2D(Collider2D other)
        // {
        //     if (!other.CompareTag(Constants.PLAYER_TAG)) return;

        //     diggable = false;
        // }
    }
}
