using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MD.CraftingSystem;

namespace MD.UI
{
    public class RecipesMenuController : MonoBehaviour
    {
        [SerializeField]
        private CraftingRecipe recipeSO = null;
        [SerializeField]
        private Image RecipeImage = null;

        [SerializeField]
        private GameObject RecipeMaterials = null;

        [SerializeField]
        private Text RecipeDescription = null;

        [SerializeField]
        private Text RecipeName = null;

        [SerializeField]
        private RecipeMenuMaterialsUIController materialsUIController = null;

        [SerializeField]
        private int currentIndex = 0;

        private int count = 0;

        void OnEnable()
        {
            currentIndex = 0;
            UpdateUI();
            count = recipeSO.CrafteditemsList.Count;
        }

        public void NextItem()
        {
            Debug.Log("next item Pressed");
            currentIndex += 1;
            if (currentIndex >= count) currentIndex=0;
            UpdateUI();
        }

        public void PreviousItem()
        {
            currentIndex -= 1;
            if (currentIndex < 0) currentIndex= count -1;
            UpdateUI();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                NextItem();
            }
        }

        
        private void UpdateUI()
        {
            materialsUIController.UpdateUI(recipeSO.CrafteditemsList[currentIndex].name);
            RecipeImage.sprite = recipeSO.CrafteditemsList[currentIndex].UISprite;
            string description = recipeSO.CrafteditemsList[currentIndex].quirk.GetDescription();
            string name = recipeSO.CrafteditemsList[currentIndex].quirk.GetName();
            RecipeDescription.text = "unknown";
            RecipeName.text = "unknown";
            if (description != "")
            {
                RecipeDescription.text = description;
            }
            if (description != "")
            {
                RecipeName.text = name;
            }

        }
    }
}