using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MD.UI
{
    public class InventoryMenuDrag : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        // [SerializeField]
        // SwipeMenu swipeMenu = null;

        // [SerializeField]
        // private float SwitchMenuPercentThreshold = .5f;
        [SerializeField]
        private float percentThreshold = .2f;

        [SerializeField]
        private float swipeSpeed = 8f;

        [SerializeField]
        private float dragSpeed = .5f;

        [SerializeField]
        private float dragMaxExceedLength = 10f;

        private int index;
        private int count = 0;

        private float swipeLength;

        private Vector3 location;
        // private Vector3 YLocation;
        private float cellSize;
        private float cellSpacing;

        // private bool isDraging;
        private RectTransform rectTransform;

        // private bool isSwitchMenu = false;

        public void OnDrag(PointerEventData data)
        {
            float XDiff = (data.pressPosition.x - data.position.x) * dragSpeed;
            XDiff = ((index == 0 && XDiff < 0) || (index == count - 1 && XDiff > 0))
                        ? Mathf.Clamp(XDiff, -dragMaxExceedLength, dragMaxExceedLength)
                        : Mathf.Clamp(XDiff, -swipeLength - dragMaxExceedLength, swipeLength + dragMaxExceedLength);

            rectTransform.anchoredPosition = location - new Vector3(XDiff, 0f, 0f);
        }

        public void OnEndDrag(PointerEventData data)
        {
            float percentage = (data.pressPosition.x - data.position.x) / cellSize;
            location = GetMovedLocation(location, percentage);
            StartCoroutine(SmoothMove(rectTransform.anchoredPosition, location));
        }

        private Vector3 GetMovedLocation(Vector3 location, float percentage)
        {
            if (Mathf.Abs(percentage) <= percentThreshold)
            {
                return location;
            }

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

            return newLocation;
        }

        private IEnumerator SmoothMove(Vector3 start, Vector3 end)
        {
            float elapsedTime = 0;
            float time = 1f / swipeSpeed;
            while (elapsedTime < time)
            {
                elapsedTime = Mathf.Min(elapsedTime + Time.deltaTime, time);
                rectTransform.anchoredPosition = Vector3.Slerp(start, end, elapsedTime * swipeSpeed);

                yield return null;
            }
        }

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            GridLayoutGroup layoutGroup = GetComponent<GridLayoutGroup>();
            swipeLength = layoutGroup.cellSize.x + layoutGroup.spacing.x;
            cellSize = layoutGroup.cellSize.x;
            cellSpacing = layoutGroup.spacing.x;
            index = 0;
            count = 0;
            // TriggerIndexChangeEvent();
            Initialize();
            SetSelectedIndex(0);
            var consumer = EventSystems.EventConsumer.GetOrAttach(gameObject);
            consumer.StartListening<AddInventoryItemData>(HandleAddItem);   
            consumer.StartListening<RemoveInventoryItemData>(HandleRemoveItem);  
        }

        private void Initialize()
        {
            // 2*halfcellsizePadding + num*cell + (num-1)*space
            rectTransform.sizeDelta = new Vector2(cellSize + count * cellSize + (count - 1) * cellSpacing, rectTransform.sizeDelta.y);
        }

        private void SetSelectedIndex(int index)
        {
            rectTransform.anchoredPosition = new Vector3(index <= 0 ? 0f : index * (cellSize + cellSpacing), 0f, 0f);
            location = rectTransform.anchoredPosition;
            TriggerIndexChangeEvent();
        }

        private void HandleAddItem(AddInventoryItemData data)
        {
            count++;
            HandleItemsNumberChange();
        }

        private void HandleRemoveItem(RemoveInventoryItemData data)
        {
            count--;
            HandleItemsNumberChange();
        }

        private void HandleItemsNumberChange()
        {
            Initialize();
            if (index >= count) index = Mathf.Max(0, count - 1);
            SetSelectedIndex(index);
        }

        private void TriggerIndexChangeEvent()
        {
            EventSystems.EventManager.Instance.TriggerEvent(new InventoryMenuIndexChangeData(index));
        }
    }
}
