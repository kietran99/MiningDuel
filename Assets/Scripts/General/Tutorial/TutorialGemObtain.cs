using MD.Diggable.Gem;
using UnityEngine;

namespace MD.Tutorial
{
    [RequireComponent(typeof(GemValue))]
    public class TutorialGemObtain : TutorialDiggableObtain
    {
        private GemValue gemValue;

        protected override void Start()
        {
            base.Start();
            gemValue = GetComponent<GemValue>();
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            EventSystems.EventManager.Instance.TriggerEvent(new DiggableContactData());
        }

        protected override void TriggerObtainEvent()
        {
            EventSystems.EventManager.Instance.TriggerEvent(new Character.ScoreChangeData(gemValue.Value));
        }
    }
}
