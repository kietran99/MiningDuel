using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD.Quirk;
using System;
using UnityEngine.UI;
namespace MD.CraftingSystem
{

    [System.Serializable]
    public struct CraftedItem
    {
        public CraftItemName name;
        public BaseQuirk quirk;
        public Sprite UISprite;
    }
    [Serializable]
    public enum CraftItemName
    {
        None = 0,
        SpeedBoost1 = 1,
        SpeedBoost2,
        Shield1,
        Shield2,
        Syringe1,
        Syringe2,
        MightyBlessing1,
        MightyBlessing2,
        DrillMachine,
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

        public int LONG_RECIPE_LENGTH = 5;

        public int SHORT_RECIPE_LENGTH = 3;

        public bool IsGemCraftable(DiggableType type) => type.IsGem();
        
        
        public Trie trie;

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

        #if UNITY_EDITOR
        public void SaveRecipes()
        {
            int numberOfValue = Enum.GetNames(typeof(CraftableGem)).Length;
            trie = new Trie(numberOfValue);
            foreach (Recipe recipe in Recipes)
            {
                trie.Insert(recipe.Materials,recipe.craftItemName);
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Save done!");
        }
        #endif
        
        public CraftItemName Search(CraftableGem[] gems)
        {
            return trie.Search(gems);
        }

        public CraftableGem[] GetMaterials(CraftItemName name)
        {
            for (int i=0; i< Recipes.Count; i++)
            {
                if (Recipes[i].craftItemName.Equals(name))
                {
                    return Recipes[i].Materials;
                }
            }
            return null;
        }
    }
}

