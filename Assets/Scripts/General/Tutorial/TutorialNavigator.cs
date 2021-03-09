using UnityEngine;
using UnityEngine.UI;

namespace MD.Tutorial
{
    public class TutorialNavigator : MonoBehaviour
    {
        #region SERIALIZE FIELDS
        [SerializeField]
        private Button nextLineButton = null;

        [SerializeField]
        private Text navigationText = null;

        [SerializeField]
        private GameObject nextLineAvailableImage = null;

        [SerializeField]
        private GameObject mask = null;

        [SerializeField]
        private Transform[] components = null;

        [SerializeField]
        private TutorialNavigateData[] navigateData = null;
        #endregion

        private TutorialState curTutorialState;
        public int loadIdx = 0;

        private void Start()
        {
            nextLineButton.onClick.AddListener(ProcessTutorialState);
            gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<TutorialStateChangeData>(UpdateView);
            LoadData(navigateData[loadIdx]);
        }

        private void OnDestroy() => nextLineButton.onClick.RemoveAllListeners();

        private void LoadData(TutorialNavigateData data)
        {
            curTutorialState = data.SetupEnvironment();
        }

        private void ProcessTutorialState()
        {      
            curTutorialState.RequestNextState();
        }

        private void UpdateView(TutorialStateChangeData stateChangeData)
        {
            // Unfocus();
            transform.SetAsLastSibling();

            if (stateChangeData.shouldToggleMask) 
            {
                mask.SetActive(!mask.activeInHierarchy);
            }
            
            PrintLine(stateChangeData.line, stateChangeData.isLastLine);
            stateChangeData.maybefocusObjectName.Match(objName => MayFocus(objName), () => {});
        }

        private void PrintLine(string line, bool isLastLine)
        {
            navigationText.text = line;
            nextLineAvailableImage.SetActive(!isLastLine);
        }

        private void MayFocus(string objToFocus)
        {
            var maybeComponent = components.LookUp(component => component.name.Equals(objToFocus));

            if (maybeComponent.idx.Equals(Constants.INVALID))
            {
                Debug.LogWarning("Cannot find any Game Object named: " + objToFocus);
                return;
            }

            Focus(maybeComponent.item);
        }

        private void ToggleMask() => mask.SetActive(mask.activeInHierarchy);

        private void Focus(Transform component)
        {
            mask.SetActive(true);
            BringToFront(component);
        }

        private void Unfocus()
        {
            mask.SetActive(false);
            transform.SetAsLastSibling();
        }

        private void BringToFront(Transform component)
        {
            component.SetSiblingIndex(transform.GetSiblingIndex() + 1);
        }
    }
}
