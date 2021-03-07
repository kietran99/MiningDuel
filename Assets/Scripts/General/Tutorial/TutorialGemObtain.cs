using MD.Diggable.Gem;
using UnityEngine;

namespace MD.Tutorial
{
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(GemValue))]
    public class TutorialGemObtain : MonoBehaviour
    {
        private GemValue gemValue;
        private bool diggable;

        private void Start()
        {
            gemValue = GetComponent<GemValue>();
            gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<DigInvokeData>(MayDig);
        }

        private void MayDig(DigInvokeData _)
        {
            if (!diggable)
            {
                return;
            }

            EventSystems.EventManager.Instance.TriggerEvent(new Character.ScoreChangeData(gemValue.Value));
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG))
            {
                return;
            }

            diggable = true;
            EventSystems.EventManager.Instance.TriggerEvent(new DiggableContactData());
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG))
            {
                return;
            }

            diggable = false;
        }
    }
}
