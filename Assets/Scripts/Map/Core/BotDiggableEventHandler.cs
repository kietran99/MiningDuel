using System.Collections.Generic;

namespace MD.Map.Core
{
    public class BotDiggableEventHandler
    {
        private Dictionary<DiggableType, System.Action<MD.AI.PlayerBot, int, DiggableType>> eventHandlerDict;

        public BotDiggableEventHandler()
        {
            eventHandlerDict = new Dictionary<DiggableType, System.Action<MD.AI.PlayerBot, int, DiggableType>>()
            {
                { DiggableType.COMMON_GEM, HandleGemDugEvent },
                { DiggableType.UNCOMMON_GEM, HandleGemDugEvent },
                { DiggableType.RARE_GEM, HandleGemDugEvent },
                { DiggableType.SUPER_RARE_GEM, HandleGemDugEvent },
                { DiggableType.NORMAL_BOMB, HandleProjectileDugEvent },
                { DiggableType.EMPTY, (digger, value, type) => { UnityEngine.Debug.Log("Bot Dug Empty Tile"); } }
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