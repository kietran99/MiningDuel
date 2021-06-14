using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MD.CraftingSystem;

namespace MD.UI
{
    public class RecipeMenuMaterialsUIController : MonoBehaviour
    {
        [SerializeField]
        private List<Image> MaterialsList = null;

        [SerializeField]
        private CraftingRecipe recipeSO = null;

        [SerializeField]
        private Sprite CommenGemSprite = null;

        [SerializeField]
        private Sprite UncommenGemSprite = null;

        [SerializeField]
        private Sprite RareGemSprite = null;

        [SerializeField]
        private Sprite SuperRareGemSprite = null;

        [SerializeField]
        private bool isInitialized =false;

        void OnEnable()
        {
            if (MaterialsList.Count == 5) 
            {
                isInitialized = true;
            }
            else
            {
                Debug.LogWarning("materials list missing Image objects");
            }
        }

        public void UpdateUI(CraftItemName name)
        {
            if (!isInitialized) 
            {
                return;
            }

            CraftableGem[] res = recipeSO.GetMaterials(name);

            if (res == null) 
            {
                return;
            }

            int currentIndex;
            for (currentIndex = 0; currentIndex < res.Length; currentIndex++)
            {
                MaterialsList[currentIndex].gameObject.SetActive(true);
                MaterialsList[currentIndex].sprite = GetSprite(res[currentIndex]);
            }

            for (int i = currentIndex; i < MaterialsList.Count; i++)
            {
                MaterialsList[i].gameObject.SetActive(false);
            }
        }

        private Sprite GetSprite(CraftableGem gem)
        {
            switch (gem)
            {
                case CraftableGem.CommenGem:
                    return CommenGemSprite;
                case CraftableGem.UncommnenGem:
                    return UncommenGemSprite;
                case CraftableGem.RareGem:
                    return RareGemSprite;
                case CraftableGem.SuperRareGem:
                    return SuperRareGemSprite;
                default:
                    return CommenGemSprite;
            }
        }
    }
}