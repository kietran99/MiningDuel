using MD.Character;
using MD.UI;
using UnityEngine;

namespace MD.Diggable.Gem
{
    [RequireComponent (typeof(CircleCollider2D))]
    [RequireComponent(typeof(GemValue))]
    public class GemObtain : MonoBehaviour
    {
        private GemValue gemValue;
        private DigAction digger;
        private bool diggable;

        void Start()
        {
            gemValue = GetComponent<GemValue>();
            diggable = false;
            DigControl.digButtonClick += Dig;
        }

        private void Dig()
        {
            if (!diggable) return;

            gemValue.DecreaseValue(digger.Power);

            if (gemValue.RemainingHit > 0) return;

            EventSystems.EventManager.Instance.TriggerEvent(new GemDigSuccessData(gemValue.Value));
            Destroy(gameObject);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG)) return;
            
            diggable = true;
            digger = other.GetComponent<DigAction>();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG)) return;

            diggable = false;
        }
    }
}
