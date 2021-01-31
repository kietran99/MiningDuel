﻿using System.Collections.Generic;

namespace MD.Map.Core
{
    public class BotDiggableEventHandler
    {
        private Dictionary<DiggableType, System.Action<MD.AI.PlayerBot, int, DiggableType>> eventHandlerDict;

        public BotDiggableEventHandler()
        {
            eventHandlerDict = new Dictionary<DiggableType, System.Action<MD.AI.PlayerBot, int, DiggableType>>()
            {
                { DiggableType.CommonGem, HandleGemDugEvent },
                { DiggableType.UncommonGem, HandleGemDugEvent },
                { DiggableType.RareGem, HandleGemDugEvent },
                { DiggableType.NormalBomb, HandleProjectileDugEvent },
                { DiggableType.Empty, (digger, value, type) => { UnityEngine.Debug.Log("Bot Dug Empty Tile"); } }
            };
        }

        private void HandleGemDugEvent(MD.AI.PlayerBot bot, int value, DiggableType type)
        {
            bot.IncreaseScore(value);
        }

        private void HandleProjectileDugEvent(MD.AI.PlayerBot bot, int value, DiggableType type)
        {
            bot.SpawnProjectile(type);
        }

        public void HandleDiggableDugEvent(MD.AI.PlayerBot bot, MD.Map.Core.ReducedData reducedData)
        {
            eventHandlerDict[reducedData.type](bot, reducedData.max, reducedData.type);
        }
    }
}