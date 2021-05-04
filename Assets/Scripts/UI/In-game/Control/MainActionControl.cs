using System;
using MD.Diggable.Projectile;
using UnityEngine;
using UnityEngine.UI;
using MD.Character;

namespace MD.UI
{
    public class MainActionControl : MonoBehaviour
    {
        [SerializeField]
        private Button button = null;

        private MainActionType curActionType;

        private System.Collections.Generic.Dictionary<MainActionType, Action> invokerDict;

        private void Start()
        {
            curActionType = MainActionType.DIG;

            invokerDict = new System.Collections.Generic.Dictionary<MainActionType, Action>()
            {
                { MainActionType.DIG, () => EventSystems.EventManager.Instance.TriggerEvent(new DigInvokeData()) },
                { MainActionType.ATTACK, () => EventSystems.EventManager.Instance.TriggerEvent(new AttackInvokeData()) }
            };

            button.onClick.AddListener(Invoke);
            var eventConsumer = EventSystems.EventConsumer.Attach(gameObject);
            EventSystems.EventManager.Instance.StartListening<ProjectileObtainData>(HideButton);
            EventSystems.EventManager.Instance.StartListening<ThrowInvokeData>(ShowButton);
            eventConsumer.StartListening<MainActionToggleData>(ToggleInvoker);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(Invoke);
            EventSystems.EventManager.Instance.StopListening<ProjectileObtainData>(HideButton);
            EventSystems.EventManager.Instance.StopListening<ThrowInvokeData>(ShowButton);
        }

        private void ToggleInvoker(MainActionToggleData actionToggleData)
        {
            curActionType = actionToggleData.actionType;
        }

        public void Invoke()
        {
            invokerDict[curActionType]();
        }

        private void ShowButton(ThrowInvokeData obj)
        {
            button.gameObject.SetActive(true);
        }

        private void HideButton(ProjectileObtainData obj)
        {
            button.gameObject.SetActive(false);
        }
    }
}
