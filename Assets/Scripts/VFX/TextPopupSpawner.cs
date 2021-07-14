﻿using UnityEngine;
using System.Collections.Generic;

namespace MD.VisualEffects
{
    public class TextPopupSpawner : MonoBehaviour
    {
        [SerializeField]
        private ObjectPool pool = null;

        [SerializeField]
        private float yOffset = 1f;

        [SerializeField]
        private Color normalDamageColor = Color.white;

        [SerializeField]
        private Color criticalDamageColor = Color.white;

        private Dictionary<int, TextPopupBlaster> cachedPopup = new Dictionary<int, TextPopupBlaster>();

        private void Start()
        {
            EventSystems.EventConsumer.Attach(gameObject).StartListening<Character.DamageGivenData>(HandleDamageGiven);
        }

        private void HandleDamageGiven(Character.DamageGivenData data)
        {
            Spawn(data.dmg, data.isCritical, new Vector2(data.damagablePos.x, data.damagablePos.y + yOffset));
        }

        private void Spawn(int dmg, bool isCritical, Vector2 spawnPos)
        {
            var popup = pool.Pop();
            var popupId = popup.GetInstanceID();

            if (!cachedPopup.TryGetValue(popupId, out var popupBlaster))
            {
                popupBlaster = popup.GetComponent<TextPopupBlaster>();
                cachedPopup.Add(popupId, popupBlaster);
                popupBlaster.OnFade += PushToPool;
            }

            popupBlaster.Blast(dmg.ToString(), spawnPos, isCritical ? criticalDamageColor : normalDamageColor);
        }

        private void PushToPool(GameObject popup)
        {
            pool.Push(popup);
        }
    }
}
