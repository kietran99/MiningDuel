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
        private readonly static string NAME_BOMB = "Bomb Stats";

        private static readonly Dictionary<DiggableType, IDiggable> typeDict = new Dictionary<DiggableType, IDiggable>
        {
            { DiggableType.CommonGem, (GemStats) Resources.Load(GEM_PATH + NAME_COMMON_GEM) },
            { DiggableType.UncommonGem, (GemStats) Resources.Load(GEM_PATH + NAME_UNCOMMON_GEM) },
            { DiggableType.RareGem, (GemStats)Resources.Load(GEM_PATH + NAME_RARE_GEM) },
            { DiggableType.NormalBomb, (ProjectileStats)Resources.Load(PROJECTILE_PATH + NAME_BOMB) }
        };

        public static IDiggable Convert(DiggableType type)
        {
            if (typeDict.TryGetValue(type, out IDiggable diggable))
            {
                return diggable;
            }

            return default;
        }
    }
}