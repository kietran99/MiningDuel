using UnityEngine;
using UnityEngine.UI;

namespace MD.Tutorial
{
    public class TutorialNavigator : MonoBehaviour
    {
        #region SERIALIZE FIELDS
        [SerializeField]
        private Transform player = null;

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
        #endregion

        private TutorialState curTutorialState;

        private void Start()
        {
            nextLineButton.onClick.AddListener(ProcessTutorialState);
            gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<TutorialStateChangeData>(UpdateView);
        }

        private void OnDestroy() => nextLineButton.onClick.RemoveAllListeners();

        public void LoadData(TutorialNavigateData data)
        {
            player.position = new Vector3(0f, .5f, 0f);
            curTutorialState = data.SetupEnvironment();
        }

        private void ProcessTutorialState()
        {    
            if (curTutorialState == null)  
            {
                Debug.Log("No Navigate Data was loaded");
                return;
            }

            curTutorialState.RequestNextState();
        }

        private void UpdateView(TutorialStateChangeData stateChangeData)
        {
            transform.SetAsLastSibling();

            if (stateChangeData.shouldToggleMask) 
            {
                mask.SetActive(!mask.activeInHierarchy);
            }
            
            PrintLine(stateChangeData.line, stateChangeData.isLastLine);
            stateChangeData.maybefocusObjectName.Match(objName => MayFocus(objName), () => {});

            if (stateChangeData.isLastLine)
            {
                // TODO: Retarded line below, replace with something less idiotic
                Destroy(GameObject.FindObjectOfType<AbstractTutorialMaterial>().gameObject);
            }
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

        private void Focus(Transform component)
        {
            mask.SetActive(true);
            BringToFront(component);
        }

        private void BringToFront(Transform component)
        {
            component.SetSiblingIndex(transform.GetSiblingIndex() + 1);
        }
    }
}
