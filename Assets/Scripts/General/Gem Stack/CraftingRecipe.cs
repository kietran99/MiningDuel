using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MD.Quirk;
namespace MD.CraftingSystem
{
    [System.Serializable]
    public struct CraftedItem
    {
        public CraftItemName name;
        public BaseQuirk quirk;
        public Sprite UISprite;
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

        public List<CraftedItem> CrafteditemsList;

        public int MAX_NO_MATERIALS = 5;

        public bool IsGemCraftable(DiggableType type) => type.IsGem();

        public Sprite GetImage(CraftItemName name)
        {
            foreach (CraftedItem item in CrafteditemsList)
            {
                if (item.name.Equals(name)) return item.UISprite;
            }
            return null;
        }

        public BaseQuirk GetItem(CraftItemName name)
        {
            foreach (CraftedItem item in CrafteditemsList)
            {
                if (item.name.Equals(name)) return item.quirk;
            }
            return null;
        }
        
    }
}

