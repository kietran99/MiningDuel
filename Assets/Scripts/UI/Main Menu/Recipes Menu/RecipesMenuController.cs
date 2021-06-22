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

        // [SerializeField]
        // private GameObject RecipeMaterials = null;

        [SerializeField]
        private Text RecipeDescription = null;

        [SerializeField]
        private Text RecipeName = null;

        [SerializeField]
        private Text currentPageText = null;

        [SerializeField]
        private RecipeMenuMaterialsUIController materialsUIController = null;

        [SerializeField]
        private Utils.UI.UISpriteAnimationControl pageTurnAnimControl = null;

        // [SerializeField]
        private int currentIndex = 0;

        private int count = 0;

        private bool shouldMoveNext;

        void OnEnable()
        {
            currentIndex = 0;
            UpdateUI();
            count = recipeSO.CrafteditemsList.Count;
        }

        private void Start() => pageTurnAnimControl.OnEnd.AddListener(OnPageTurnEnd);

        public void NextItemCallback() // Editor ref
        {
            shouldMoveNext = true;
            pageTurnAnimControl.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            pageTurnAnimControl.Play();
        }

        public void PrevItemCallback() // Editor ref
        {
            shouldMoveNext = false;
            pageTurnAnimControl.transform.eulerAngles = new Vector3(0f, 180f, 0f);
            pageTurnAnimControl.Play();
        }

        private void OnPageTurnEnd()
        {
            if (shouldMoveNext)
            {
                NextItem();
            }
            else
            {
                PreviousItem();
            }
        }

        private void NextItem()
        {
            if (++currentIndex >= count) 
            {
                currentIndex = 0;
            }

            UpdateUI();
        }

        private void PreviousItem()
        {
            if (--currentIndex < 0) 
            {
                currentIndex = count - 1;
            }

            UpdateUI();
        }

    #if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                NextItem();
            }
        }
    #endif

        private void UpdateUI()
        {
            materialsUIController.UpdateUI(recipeSO.CrafteditemsList[currentIndex].name);
            RecipeImage.sprite = recipeSO.CrafteditemsList[currentIndex].UISprite;
            string description = recipeSO.CrafteditemsList[currentIndex].quirk.GetDescription();
            string name = recipeSO.CrafteditemsList[currentIndex].quirk.GetName();
            RecipeDescription.text = "unknown";
            RecipeName.text = "unknown";

            if (description != string.Empty)
            {
                RecipeDescription.text = description;
            }

            if (description != string.Empty)
            {
                RecipeName.text = name;
            }

            currentPageText.text = (currentIndex + 1).FormatNum();
        }
    }
}