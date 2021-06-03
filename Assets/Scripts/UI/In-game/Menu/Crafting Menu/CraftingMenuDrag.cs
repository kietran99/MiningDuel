using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MD.UI
{
    public class CraftingMenuDrag : MonoBehaviour, IDragHandler, IEndDragHandler
    {

        [SerializeField]
        RectTransform swipeMenuTransform = null;

        [SerializeField]
        private float autoSwitchMenuThreshold = 1f;

        [SerializeField]
        private float percentThreshold = .5f;

        [SerializeField]
        private float swipeSpeed = 8f;

        [SerializeField]
        private float dragSpeed = .5f;

        [SerializeField]
        private float dragMaxExceedLength = 10f;

        private int index;

        private int count = 0;
        SwipeMenu swipeMenu = null;

        private float swipeLength;

        private Vector3 location;
        // private Vector3 YLocation;
        private float cellSize;
        private float cellSpacing;
        private RectTransform rectTransform;

        private Vector3 shownPosition = Vector3.zero;
        private float maxDragLength;
        private bool isSwitchMenu = false;   
        private bool isSwiping = false;

        public void OnDrag(PointerEventData data)
        {
            if (!isSwiping)
            {
                float YDiff = (data.pressPosition.y - data.position.y) * dragSpeed;
                if (YDiff < - .2f * cellSize) // move menu to hand curent drag point position; 
                {
                    isSwitchMenu = true;
                    YDiff = Mathf.Clamp(YDiff, -maxDragLength , 0f);
                    Debug.Log("diff " + YDiff);
                    swipeMenuTransform.anchoredPosition = shownPosition - new Vector3(0f, YDiff, 0f);
                }
            }

            if (isSwitchMenu)
            {
                return;
            }
            
            float difference = (data.pressPosition.x - data.position.x) * dragSpeed;

            if (Mathf.Abs(difference) <= .1f * cellSize) 
            {
                return;
            }
            
            isSwiping = true;
            difference = (index == 0 && difference < 0f) || (index == count - 1 && difference > 0f)
                            ? Mathf.Clamp(difference, -dragMaxExceedLength, dragMaxExceedLength)
                            : Mathf.Clamp(difference, -swipeLength - dragMaxExceedLength, swipeLength + dragMaxExceedLength);

            rectTransform.anchoredPosition = location - new Vector3(difference, 0f, 0f);      
        }

        public void OnEndDrag(PointerEventData data)
        {
            if (isSwitchMenu)
            {
                float Ypercentage= (data.pressPosition.y - data.position.y) / cellSize;
                if (Ypercentage < autoSwitchMenuThreshold)
                {
                    swipeMenu.SwitchMenu();
                }
                else
                {
                    swipeMenu.ReturnToCurrentPostion();
                }
            }

            if (isSwiping)
            {
                float percentage = (data.pressPosition.x - data.position.x) / cellSize;
                if (Mathf.Abs(percentage) > percentThreshold)
                {
                    Vector3 newLocation = location;
                    if (percentage > 0f && index < count - 1)
                    {
                        newLocation += new Vector3(-swipeLength, 0f, 0f);
                        index++;
                        TriggerIndexChangeEvent();
                    }
                    else if (percentage < 0f && index > 0)
                    {
                        newLocation += new Vector3(swipeLength, 0f, 0f);
                        index--;
                        TriggerIndexChangeEvent();             
                    }
   
                    StartCoroutine(SmoothMove(rectTransform.anchoredPosition ,newLocation));
                    location = newLocation;
                }
                else
                {
                    StartCoroutine(SmoothMove(rectTransform.anchoredPosition , location));
                }   
            }

            isSwiping = false;
            isSwitchMenu = false;
        }

        private IEnumerator SmoothMove(Vector3 start, Vector3 end)
        {
            float elapsedTime = 0;
            float time = 1f / swipeSpeed;
            while (elapsedTime < time)
            {
                elapsedTime = Mathf.Min(elapsedTime + Time.deltaTime, time);
                rectTransform.anchoredPosition  = Vector3.Slerp(start, end, elapsedTime * swipeSpeed);

                yield return null;
            }
        }

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            GridLayoutGroup layoutGroup = GetComponent<GridLayoutGroup>();
            swipeLength = layoutGroup.cellSize.x + layoutGroup.spacing.x;
            cellSize = layoutGroup.cellSize.x;
            cellSpacing = layoutGroup.spacing.x;
            index = 0;
            Initialize();
            SetSelectedIndex(0);
            var eventConsumer = EventSystems.EventConsumer.GetOrAttach(gameObject);
            eventConsumer.StartListening<CraftItemsNumberChangeData>(HandleItemsNumberChange);  

            swipeMenu = swipeMenuTransform.GetComponent<SwipeMenu>();
            shownPosition = swipeMenu.GetCraftMenuLocation();
            maxDragLength = cellSize * .7f;
        }

        private void Initialize()
        {
            // 3*cellsizePadding + num*cell + (num-1)*space
            rectTransform.sizeDelta = new Vector2(3 * cellSize + count * cellSize + (count - 1) * cellSpacing, rectTransform.sizeDelta.y);
        }

        private void HandleItemsNumberChange(CraftItemsNumberChangeData data)
        {
            count = data.numOfItems;
            Initialize();
            if (index >= count) 
            {
                index = Mathf.Max(0, count - 1);
            }
            SetSelectedIndex(index);
        }

        private void SetSelectedIndex(int index)
        {
            rectTransform.anchoredPosition = index <= 0 ? Vector3.zero : new Vector3(-index * (cellSize + cellSpacing), 0f, 0f);
            location = rectTransform.anchoredPosition;
            TriggerIndexChangeEvent();
        }      

        public void TriggerIndexChangeEvent()
        {
            EventSystems.EventManager.Instance.TriggerEvent(new CraftMenuChangeIndexData(index));
        }
    }
}
