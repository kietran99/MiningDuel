using UnityEngine;
using UnityEngine.UI;

namespace MD.Tutorial
{
    public class TutorialNavigator : MonoBehaviour
    {
        #region SERIALIZE FIELDS
        [SerializeField]
        private Transform[] components = null;

        [SerializeField]
        private TutorialNavigateData[] navigateData = null;

        [SerializeField]
        private Button nextLineButton = null;

        [SerializeField]
        private Text navigationText = null;

        [SerializeField]
        private GameObject nextLineAvailableImage = null;

        [SerializeField]
        private GameObject mask = null;
        #endregion

        private TutorialNavigateData curNavData;
        private int curLineIdx = 0;

        private void Start()
        {
            nextLineButton.onClick.AddListener(HandleNavContainerClick);
            LoadData(navigateData[1]);
        }

        private void OnDestroy() => nextLineButton.onClick.RemoveAllListeners();

        private void LoadData(TutorialNavigateData data)
        {
            curLineIdx = 0;
            curNavData = data;
            HandleNavContainerClick();
        }

        private void HandleNavContainerClick()
        {
            if (curLineIdx >= curNavData.Lines.Length)
            {
                return;
            }

            Unfocus();
            PrintLine(curNavData.Lines[curLineIdx], curLineIdx == (curNavData.Lines.Length - 1));
            MayFocus();
            curLineIdx++;
        }

        private void PrintLine(string line, bool isLastLine)
        {
            navigationText.text = line;
            nextLineAvailableImage.SetActive(!isLastLine);
        }

        private void MayFocus()
        {
            var maybeFocusLine = curNavData.FocusLines.LookUp(line => line.lineIdx == curLineIdx);

            if (maybeFocusLine.idx.Equals(Constants.INVALID))
            {
                return;
            }

            var maybeComponent = components.LookUp(component => component.name.Equals(maybeFocusLine.item.gameObjectName));

            if (maybeComponent.idx.Equals(Constants.INVALID))
            {
                Debug.LogWarning("Cannot find any Game Object named: " + maybeFocusLine.item.gameObjectName);
                return;
            }

            Focus(maybeComponent.item);
        }

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

        private void BringToBack(Transform component)
        {
            component.SetSiblingIndex(transform.GetSiblingIndex() - 1);
        }
    }
}
