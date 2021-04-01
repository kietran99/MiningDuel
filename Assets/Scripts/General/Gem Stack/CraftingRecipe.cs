using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD.Diggable;

namespace MD.CraftingSystem
{
    [System.Serializable]
    public abstract class CraftedItem
    {

    }
    public enum CraftItemName
    {
        SpeedPotion1 = 0,
        SpeedPotion2,
        Shield1,
        Shield2,
        SonarUpgrade1,
        SonarUpgrade2,
        DigPotion1,
        DigPotion2,
        AvatarPotion1,
        AvatarPotion2
    }
    public enum CraftableGem
    {
        CommenGem =  DiggableType.COMMON_GEM,
        UncommnenGem = DiggableType.UNCOMMON_GEM ,
        RareGem = DiggableType.RARE_GEM,
        SuperRareGem = DiggableType.SUPER_RARE_GEM
    }

    [System.Serializable]
    public struct Recipe
    {
        public CraftableGem[] Materials;
        public CraftItemName craftItemName;
    }

    [CreateAssetMenu]
    public class CraftingRecipe : ScriptableObject
    {
        public List<Recipe> Recipes;

        public int MAX_NO_MATERIALS = 5;
    }
}

