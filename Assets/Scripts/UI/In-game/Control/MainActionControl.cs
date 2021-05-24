﻿using System;
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
            eventConsumer.StartListening<ProjectileObtainData>(HideButton);
            eventConsumer.StartListening<ThrowInvokeData>(ShowButton);
            eventConsumer.StartListening<MainActionToggleData>(ToggleInvoker);
            eventConsumer.StartListening<GetCounteredData>(TemporaryDisableButton);
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

        private void HideButton(ProjectileObtainData data)
        {
            if (data.type.Equals(DiggableType.NORMAL_BOMB))
            {
                button.gameObject.SetActive(false);
            }
        }
    }
}
