using UnityEngine;
using UnityEngine.UI;

namespace MD.Tutorial
{
    public class TutorialMenuController : MonoBehaviour
    {
        [System.Serializable]
        public struct ChapterEntry
        {
            public Button button;
            public TutorialNavigateData tutorialNavigateData;
        }

        #region SERIALIZE FIELDS
        [SerializeField]
        private TutorialNavigator navigator = null;

        [SerializeField]
        private Button menuToggleButton = null, exitButton = null;

        [Mirror.Scene]
        [SerializeField]
        private string mainMenuScene = string.Empty;

        [SerializeField]
        private Image menuToggleButtonImage = null;

        [SerializeField]
        private Sprite menuActiveSprite = null, menuUnactiveSprite = null;

        [SerializeField]
        private GameObject navigateDialogueContainer = null, mask = null, chapterButtonsContainer = null;

        [SerializeField]
        private ChapterEntry[] chapterMapper = null;
        #endregion

        private bool isMenuActive, formerMaskState;

        void Start()
        {
            isMenuActive = true;
            menuToggleButtonImage.sprite = menuUnactiveSprite;
            menuToggleButton.onClick.AddListener(ToggleMenu);
            exitButton.onClick.AddListener(Exit);
            //gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<TutorialStateChangeData>(EnableIfLastLine);
            chapterMapper.ForEach(entry => entry.button.onClick.AddListener(() => LoadTutorialChapter(entry.tutorialNavigateData)));
            EnableMenu();
        }

        // private void EnableIfLastLine(TutorialStateChangeData stateChangeData)
        // {
        //     if (!stateChangeData.isLastLine)
        //     {
        //         return;
        //     }

        //     isMenuActive = true;
        //     EnableMenu();
        // }

        private void OnDestroy()
        {
            menuToggleButton.onClick.RemoveListener(ToggleMenu);
            exitButton.onClick.RemoveListener(Exit);
            chapterMapper.ForEach(entry => entry.button.onClick.RemoveAllListeners());
        }

        private void LoadTutorialChapter(TutorialNavigateData navigateData)
        {
            isMenuActive = false;
            DisableMenu();
            formerMaskState = true;
            mask.SetActive(true);
            navigator.LoadData(navigateData);
        }

        private void ToggleMenu()
        {
            isMenuActive = !isMenuActive;
            if (isMenuActive)
            {
                EnableMenu();
            }
            else
            {
                DisableMenu();
            }
        }

        private void EnableMenu()
        {
            menuToggleButtonImage.sprite = menuActiveSprite;
            chapterButtonsContainer.SetActive(true);
            navigateDialogueContainer.SetActive(false);
            exitButton.gameObject.SetActive(true);
            formerMaskState = mask.activeInHierarchy;
            mask.SetActive(true);
        }

        private void DisableMenu()
        {
            menuToggleButtonImage.sprite = menuUnactiveSprite;
            chapterButtonsContainer.SetActive(false);
            navigateDialogueContainer.SetActive(true);
            exitButton.gameObject.SetActive(false);
            mask.SetActive(formerMaskState);
        }

        private void Exit()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuScene);
        }
    }
}
