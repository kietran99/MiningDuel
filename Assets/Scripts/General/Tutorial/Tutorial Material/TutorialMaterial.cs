using UnityEngine;
using EventSystems;
using System.Collections.Generic;

namespace MD.Tutorial
{
    public abstract class AbstractTutorialMaterial : MonoBehaviour
    {
        [SerializeField]
        protected int[] triggerLineIndices = null;

        public List<int> TriggerLineIndices => new List<int>(triggerLineIndices);

        protected void TriggerEvent(int idx) => EventSystems.EventManager.Instance.TriggerEvent(new TutorialTriggerData(triggerLineIndices[idx]));
    }

    public class TutorialMaterial<T> : AbstractTutorialMaterial where T : IEventData
    {
        private void Start()
        {
            if (triggerLineIndices.Length != 1)
            {
                Debug.LogError("There must be exactly " + 1 + " trigger indices");
                return;
            }
            
            gameObject.AddComponent<EventConsumer>().StartListening<T>(HandleEvent);
        }

        protected virtual void HandleEvent(T _) => TriggerEvent(0);
    }

    public class TutorialMaterial<T0, T1> : AbstractTutorialMaterial where T0 : IEventData where T1 : IEventData
    {
        private void Start()
        {
            if (triggerLineIndices.Length != 2)
            {
                Debug.LogError("There must be exactly " + 2 + " trigger indices");
                return;
            }
            
            var eventConsumer = gameObject.AddComponent<EventConsumer>();
            eventConsumer.StartListening<T0>(HandleEventT0);
            eventConsumer.StartListening<T1>(HandleEventT1);
        }

        protected virtual void HandleEventT0(T0 _) => TriggerEvent(0);

        protected virtual void HandleEventT1(T1 _) => TriggerEvent(1);
    }

    public class TutorialMaterial<T0, T1, T2> : AbstractTutorialMaterial where T0 : IEventData where T1 : IEventData where T2 : IEventData
    {
        private void Start()
        {
            if (triggerLineIndices.Length != 3)
            {
                Debug.LogError("There must be exactly " + 3 + " trigger indices");
                return;
            }
            
             var eventConsumer = gameObject.AddComponent<EventConsumer>();
            eventConsumer.StartListening<T0>(HandleEventT0);
            eventConsumer.StartListening<T1>(HandleEventT1);
            eventConsumer.StartListening<T2>(HandleEventT2);
        }

        protected virtual void HandleEventT0(T0 _) => TriggerEvent(0);

        protected virtual void HandleEventT1(T1 _) => TriggerEvent(1);

        protected virtual void HandleEventT2(T2 _) => TriggerEvent(2);
    }
}
