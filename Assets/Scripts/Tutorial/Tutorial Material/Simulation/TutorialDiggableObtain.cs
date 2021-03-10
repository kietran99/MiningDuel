using UnityEngine;

namespace MD.Tutorial
{
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class TutorialDiggableObtain : MonoBehaviour
    {
        private bool isDiggable;

        protected virtual void Start()
        {
            gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<DigInvokeData>(MayDig);
        }

        private void MayDig(DigInvokeData _)
        {
            if (!isDiggable)
            {
                return;
            }

            TriggerObtainEvent();
            gameObject.SetActive(false);
        }

        protected abstract void TriggerObtainEvent();

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG))
            {
                return;
            }

            isDiggable = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG))
            {
                return;
            }

            isDiggable = false;
        }
    }
}
