using System.Collections;
using UnityEngine;

namespace MD.UI
{
    public class SwipeMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject menuNameContainer = null;

        [SerializeField]
        private float moveSpeed = 1f;
        
        [SerializeField]
        private float hiddenPositionYMargin = 20f;

        [SerializeField]
        float moveExceedTime = .1f;

        [SerializeField]
        float moveExceedLength = 10f;

        private RectTransform rectTransform;

        [SerializeField]
        bool IsInventoryMenu = true;

        private readonly string INVENTORY_STRING = "Inventory";
        private readonly string CRAFT_STRING = "Craft";

        private float hiddenPositionY = 0f, showYPositionY = 0f;
        private string menuName;

        private IEnumerator SmoothSwitchMenu(float YStart, float YEnd)
        {
            float elapsedTime = 0f;
            // float time = 1f/moveSpeed;
            float firstMoveTime = (1f - moveExceedTime) / moveSpeed;
            float secondMoveTime = moveExceedTime / moveSpeed;
            Vector3 start = new Vector3(0f, YStart, 0f);
            Vector3 end = new Vector3(0f, YEnd, 0f);
            Vector3 exceedEnd = new Vector3(0f, YEnd + (YEnd > YStart ? moveExceedLength : -moveExceedLength), 0f);

            float YExceedEnd = YEnd + (YEnd > YStart ? moveExceedLength: -moveExceedLength);

            Debug.Log("Smooth move from start " + YStart + " end " + YEnd + " exceedEnd" + YExceedEnd);

            while (elapsedTime < firstMoveTime)
            {
                elapsedTime = Mathf.Min(elapsedTime + Time.deltaTime, firstMoveTime);
                // transform.localPosition = Vector3.Slerp(start,exceedEnd,elapsedTime/firstMoveTime);
                rectTransform.anchoredPosition = new Vector3(0,Mathf.Lerp(YStart,YExceedEnd,elapsedTime/firstMoveTime));

                yield return null;
            }

            elapsedTime = 0;

            while (elapsedTime < secondMoveTime)
            {
                elapsedTime = Mathf.Min(elapsedTime + Time.deltaTime, secondMoveTime);
                // transform.localPosition = Vector3.Slerp(exceedEnd,end,elapsedTime/secondMoveTime);
                rectTransform.anchoredPosition = new Vector3(0,Mathf.Lerp(YExceedEnd,YEnd,elapsedTime/secondMoveTime));

                yield return null;
            }

            menuNameContainer.SetActive(true);
            menuNameContainer.GetComponentInChildren<UnityEngine.UI.Text>().text = menuName;
        }

        public void SwitchMenu()
        {
            if (IsInventoryMenu) //switch to craft menu
            {
                menuNameContainer.SetActive(false);
                menuName = CRAFT_STRING;
                StartCoroutine(SmoothSwitchMenu(rectTransform.anchoredPosition.y, showYPositionY));
            }
            else
            {
                //turn off craft button if is on when switch back to inventory
                EventSystems.EventManager.Instance.TriggerEvent(new SetCraftButtonData(false));

                menuNameContainer.SetActive(false);
                menuName = INVENTORY_STRING;
                StartCoroutine(SmoothSwitchMenu(rectTransform.anchoredPosition.y, hiddenPositionY));
            }

            EventSystems.EventManager.Instance.TriggerEvent(new MenuSwitchEvent(!IsInventoryMenu));
            IsInventoryMenu = !IsInventoryMenu;
        }
        
        public void ReturnToCurrentPostion()
        {
            StartCoroutine(SmoothSwitchMenu(rectTransform.anchoredPosition.y, IsInventoryMenu ? hiddenPositionY : showYPositionY));
        }

        public Vector3 GetCraftMenuLocation()
        {
            return new Vector3(0f, showYPositionY, 0f);
        }

        public Vector3 GetHiddenMenuLocation()
        {
            return new Vector3(0f, hiddenPositionY, 0f);
        }

        private void SwitchMenuAfterCraft(UseItemInvokeData data)
        {
            if (!IsInventoryMenu)
            {
                SwitchMenu();
            }
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            hiddenPositionY = rectTransform.rect.height + hiddenPositionYMargin;
            showYPositionY = 0f;
        }

        void Start()
        {
            EventSystems.EventConsumer.GetOrAttach(gameObject).StartListening<UseItemInvokeData>(SwitchMenuAfterCraft);
        }
    }
}
