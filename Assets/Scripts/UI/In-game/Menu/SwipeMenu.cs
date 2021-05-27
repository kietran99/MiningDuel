﻿using System.Collections;
using UnityEngine;

namespace MD.UI
{
    public class SwipeMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject MenuNameObj = null;

        [SerializeField]
        private CraftingMenuUI craftingMenuUI = null;

        [SerializeField]
        private float moveSpeed = 1f;
        
        [SerializeField]
        private float hiddenPositionYMargin = 20f;
        float HiddenPositionY = 0f;
        float ShowYPositionY = 0f;

        [SerializeField]
        float moveExceedTime = .1f;

        [SerializeField]
        float moveExceedLength = 10f;

        private RectTransform rectTransform;

        [SerializeField]
        bool IsInventoryMenu = true;

        private string INVENTORY_STRING = "Inventory";
        private string CRAFT_STRING = "Craft";

        private string menuName;

        private IEnumerator SmoothSwitchMenu(float YStart, float YEnd)
        {
            float elapsedTime = 0;
            // float time = 1f/moveSpeed;
            float firstMoveTime = (1f - moveExceedTime)/moveSpeed;
            float secondMoveTime = moveExceedTime/moveSpeed;
            Vector3 start = new Vector3 (0, YStart,0);
            Vector3 end = new Vector3 (0, YEnd, 0);
            Vector3 exceedEnd = new Vector3 (0, YEnd + (YEnd>YStart?moveExceedLength:-moveExceedLength),0);

            float YExceedEnd = YEnd + (YEnd>YStart?moveExceedLength:-moveExceedLength);

            Debug.Log("smooth move from start " + YStart + " end " + YEnd + " exceedEnd" + YExceedEnd );
            while (elapsedTime < firstMoveTime)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= firstMoveTime) elapsedTime = firstMoveTime;
                
                // transform.localPosition = Vector3.Slerp(start,exceedEnd,elapsedTime/firstMoveTime);
                rectTransform.anchoredPosition = new Vector3(0,Mathf.Lerp(YStart,YExceedEnd,elapsedTime/firstMoveTime));
                yield return null;
            }
            elapsedTime = 0;
            while(elapsedTime < secondMoveTime)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= secondMoveTime) elapsedTime = secondMoveTime;
                // transform.localPosition = Vector3.Slerp(exceedEnd,end,elapsedTime/secondMoveTime);
                rectTransform.anchoredPosition = new Vector3(0,Mathf.Lerp(YExceedEnd,YEnd,elapsedTime/secondMoveTime));
                yield return null;
            }

            MenuNameObj.SetActive(true);
            MenuNameObj.GetComponentInChildren<UnityEngine.UI.Text>().text = menuName;
        }

        public void SwitchMenu()
        {
            if(IsInventoryMenu) //switch to craft menu
            {
                MenuNameObj.SetActive(false);
                menuName = CRAFT_STRING;
                StartCoroutine(SmoothSwitchMenu(rectTransform.anchoredPosition.y,ShowYPositionY));
            }
            else
            {
                //turn off craft button if is on when switch back to inventory
                EventSystems.EventManager.Instance.TriggerEvent(new SetCraftButtonData(false));

                MenuNameObj.SetActive(false);
                menuName = INVENTORY_STRING;
                StartCoroutine(SmoothSwitchMenu(rectTransform.anchoredPosition.y,HiddenPositionY));
            }
            EventSystems.EventManager.Instance.TriggerEvent(new MenuSwitchEvent(!IsInventoryMenu));
            IsInventoryMenu = !IsInventoryMenu;
        }
        
        public void ReturnToCurrentPostion()
        {
            if (IsInventoryMenu)
            {
                StartCoroutine(SmoothSwitchMenu(rectTransform.anchoredPosition.y,HiddenPositionY));
            }
            else
            {
                StartCoroutine(SmoothSwitchMenu(rectTransform.anchoredPosition.y,ShowYPositionY));
            }
        }

        public Vector3 GetCraftMenuLocation()
        {
            return new Vector3(0f,ShowYPositionY, 0f);
        }
        public Vector3 GetHiddentMenuLocation()
        {
            return new Vector3(0f,HiddenPositionY, 0f);
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            HiddenPositionY = rectTransform.rect.height + hiddenPositionYMargin;
            ShowYPositionY = 0f;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SwitchMenu();
            }
        }
    }
}