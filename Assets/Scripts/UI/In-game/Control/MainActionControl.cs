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
        private MainActionType lastActionType;
        private bool isStunned = false;

        private System.Collections.Generic.Dictionary<MainActionType, Action> invokerDict;

        private void Start()
        {
            curActionType = MainActionType.DIG;
            lastActionType = curActionType;

            invokerDict = new System.Collections.Generic.Dictionary<MainActionType, Action>()
            {
                { MainActionType.DIG, () => EventSystems.EventManager.Instance.TriggerEvent(new DigInvokeData()) },
                { MainActionType.ATTACK, () => EventSystems.EventManager.Instance.TriggerEvent(new AttackInvokeData()) },
                { MainActionType.SETTRAP, () => EventSystems.EventManager.Instance.TriggerEvent(new SetTrapInvokeData())}
            };

            button.onClick.AddListener(Invoke);
            var eventConsumer = EventSystems.EventConsumer.Attach(gameObject);
            eventConsumer.StartListening<ProjectileObtainData>(HideButton);
            eventConsumer.StartListening<ThrowInvokeData>(ShowButton);
            eventConsumer.StartListening<MainActionToggleData>(ToggleInvoker);
            eventConsumer.StartListening<GetCounteredData>(TemporaryDisableButton);
            eventConsumer.StartListening<StunStatusData> (HandleStunStatusChange);
        }

        private void TemporaryDisableButton(Character.GetCounteredData counterData)
        {
            button.interactable = false;
            Invoke(nameof(EnableButton), counterData.immobilizeTime);
        }

        private void EnableButton() => button.interactable = true;

        private void OnDestroy()
        {
            button.onClick.RemoveListener(Invoke);
        }

        private void ToggleInvoker(MainActionToggleData actionToggleData)
        {
            //if switch to attack
            //store current action type when switch to attack
            if (actionToggleData.actionType == MainActionType.ATTACK && curActionType != MainActionType.ATTACK)
            {
                lastActionType = curActionType;
                curActionType = MainActionType.ATTACK;
            }
            //if return
            //return to last action type
            else if(curActionType == MainActionType.ATTACK && actionToggleData.actionType != MainActionType.ATTACK)
            {
                curActionType = lastActionType;
            }
            else
                curActionType = actionToggleData.actionType;
        }

        public void Invoke()
        {
            if (isStunned) return;
            invokerDict[curActionType]();
        }

        private void ShowButton(ThrowInvokeData obj)
        {
            button.gameObject.SetActive(true);
        }

        private void HideButton(ProjectileObtainData data)
        {
            if (data.type.Equals(DiggableType.NORMAL_BOMB))
            {
                button.gameObject.SetActive(false);
            }
        }

        private void HandleStunStatusChange(StunStatusData data) => isStunned = data.isStunned;
    }
}
