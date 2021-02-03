using MD.Diggable.Gem;
using MD.Diggable.Projectile;
using System.Collections.Generic;
using UnityEngine;

namespace MD.Diggable.Core
{
    public static class DiggableTypeConverter
    {
        private readonly static string GEM_PATH = "Gem/";
        private readonly static string PROJECTILE_PATH = "Projectile/";

        private readonly static string NAME_COMMON_GEM = "Common Gem Stats";
        private readonly static string NAME_UNCOMMON_GEM = "Uncommon Gem Stats";
        private readonly static string NAME_RARE_GEM = "Rare Gem Stats";
        private readonly static string NAME_SUPER_RARE_GEM = "Super Rare Gem Stats";
        private readonly static string NAME_BOMB = "Bomb Stats";

        private static readonly Dictionary<DiggableType, IDiggableStats> typeDict = new Dictionary<DiggableType, IDiggableStats>
        {
            { DiggableType.COMMON_GEM, (GemStats) Resources.Load(GEM_PATH + NAME_COMMON_GEM) },
            { DiggableType.UNCOMMON_GEM, (GemStats) Resources.Load(GEM_PATH + NAME_UNCOMMON_GEM) },
            { DiggableType.RARE_GEM, (GemStats)Resources.Load(GEM_PATH + NAME_RARE_GEM) },
            { DiggableType.SUPER_RARE_GEM, (GemStats)Resources.Load(GEM_PATH + NAME_SUPER_RARE_GEM) },
            { DiggableType.NORMAL_BOMB, (ProjectileStats)Resources.Load(PROJECTILE_PATH + NAME_BOMB) }
        };

        private static readonly Dictionary<int, DiggableType> valToTypeDict = new Dictionary<int, DiggableType>
        {
            { 1, DiggableType.COMMON_GEM },
            { 4, DiggableType.UNCOMMON_GEM },
            { 10, DiggableType.RARE_GEM },
            { 20, DiggableType.SUPER_RARE_GEM },
            { -1, DiggableType.NORMAL_BOMB }
        };

        public static IDiggableStats Convert(DiggableType type)
        {
            if (typeDict.TryGetValue(type, out IDiggableStats diggable))
            {
                return diggable;
            }

            return default;
        }

        public static IDiggableStats Convert(int gemValue)
        {
            if (!valToTypeDict.TryGetValue(gemValue, out DiggableType type)) 
            {
                return default;
            }

            if (typeDict.TryGetValue(type, out IDiggableStats diggable))
            {
                return diggable;
            }

            return default;
        }
    }
}