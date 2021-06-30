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
        
        [SerializeField]
        private Image buttonImage = null;

        [SerializeField]
        private Sprite AttackActionSprite = null;

        [SerializeField]
        private Sprite DigActionSprite = null;
        
        [SerializeField]
        private Sprite SetTrapActionSprite = null;

        private MainActionType curActionType;

        private System.Collections.Generic.Dictionary<MainActionType, Action> invokerDict;

        private void Start()
        {
            curActionType = MainActionType.DIG;

            invokerDict = new System.Collections.Generic.Dictionary<MainActionType, Action>()
            {
                { MainActionType.DIG, () => EventSystems.EventManager.Instance.TriggerEvent(new DigInvokeData()) },
                { MainActionType.ATTACK, () => EventSystems.EventManager.Instance.TriggerEvent(new AttackInvokeData()) },
                { MainActionType.SETTRAP, () => EventSystems.EventManager.Instance.TriggerEvent(new SetTrapInvokeData()) }
            };

            button.onClick.AddListener(Invoke);
            var eventConsumer = EventSystems.EventConsumer.Attach(gameObject);
            eventConsumer.StartListening<ProjectileObtainData>(HideButton);
            eventConsumer.StartListening<ThrowInvokeData>(ShowButton);
            eventConsumer.StartListening<MainActionToggleData>(ToggleInvoker);
            eventConsumer.StartListening<StunStatusData>(HandleStunStatusChange);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(Invoke);
        }

        private void ToggleInvoker(MainActionToggleData actionToggleData)
        {
            curActionType = actionToggleData.actionType;
            buttonImage.sprite = GetSprite();
        }

        public void Invoke()
        {
            invokerDict[curActionType]();
        }

        private void ShowButton(ThrowInvokeData obj)
        {
            button.gameObject.SetActive(true);
        }

        private void HideButton(ProjectileObtainData data)
        {
            if (data.type == DiggableType.NORMAL_BOMB)
            {
                button.gameObject.SetActive(false);
            }
        }

        private void HandleStunStatusChange(StunStatusData data)
        {
            button.interactable = !data.isStunned;
        }

        private Sprite GetSprite()
        {
            switch (curActionType)
            {
                case MainActionType.ATTACK:
                    return AttackActionSprite;
                case MainActionType.DIG:
                    return DigActionSprite;
                case MainActionType.SETTRAP:
                    return SetTrapActionSprite;
                default:
                    return DigActionSprite;
            }
        }
    }
}
