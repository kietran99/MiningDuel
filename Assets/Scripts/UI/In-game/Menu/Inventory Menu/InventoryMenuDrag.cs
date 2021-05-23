using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryMenuDrag : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private float PercentThreshold = .2f;

    [SerializeField]
    private float SwipeSpeed = 1f;

    [SerializeField]

    private float dragSpeed = .5f;

    [SerializeField]
    private float dragMaxExceedLength = 10f;

    [SerializeField]
    private int index;

    [SerializeField]
    private int count = 0;

    private float swipeLength;

    private Vector3 location;
    private float cellSize;
    private float cellSpacing;

    private bool isDraging;
    private RectTransform rectTransform;

    public void OnDrag(PointerEventData data)
    {
        Debug.Log("OnDrag");
        float difference = (data.pressPosition.x - data.position.x)*dragSpeed;
        if ((index == 0 && difference < 0) ||  (index == count-1 && difference > 0 ))
        {
            difference =  Mathf.Clamp(difference,-dragMaxExceedLength,dragMaxExceedLength);
        }
        else
        {
            difference =  Mathf.Clamp(difference,-swipeLength -dragMaxExceedLength, swipeLength + dragMaxExceedLength);
        }
        rectTransform.anchoredPosition  = location - new Vector3(difference,0,0);
    }

    public void OnEndDrag(PointerEventData data)
    {
        float percentage = (data.pressPosition.x - data.position.x)/cellSize;
        if (Mathf.Abs(percentage) > PercentThreshold)
        {
            Vector3 newLocation = location;
            if (percentage > 0 && index < count -1)
            {
                newLocation += new Vector3(-swipeLength,0,0);
                index ++;
                TriggerIndexChangeEvent();
            }
            else if (percentage < 0 && index > 0)
            {
                newLocation += new Vector3(swipeLength,0,0);
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

    private IEnumerator SmoothMove(Vector3 start, Vector3 end)
    {
        float elapsedTime = 0;
        float time = 1f/SwipeSpeed;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= time) elapsedTime = time;
            rectTransform.anchoredPosition  = Vector3.Slerp(start,end,elapsedTime*SwipeSpeed);
            yield return null;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        GridLayoutGroup glg = GetComponent<GridLayoutGroup>();
        swipeLength = glg.cellSize.x + glg.spacing.x;
        cellSize = glg.cellSize.x;
        cellSpacing = glg.spacing.x;
        index =0;
        count =0;
        // TriggerIndexChangeEvent();
        Initialize();
        SetSelectedIndex(0);

        var consumer = gameObject.AddComponent<EventSystems.EventConsumer>();
        consumer.StartListening<AddInventoryItemData>(HandleAddItem);   
        consumer.StartListening<RemoveInventoryItemData>(HandleRemoveItem);  
    }

    private void Initialize()
    {
        rectTransform.sizeDelta = new Vector2(count*cellSize + (count-1)*cellSpacing,rectTransform.sizeDelta.y);
        // rectTransform.sizeDelta = new Vector2(count*cellSize + (count-1)*cellSpacing,0);
    }

    private void SetSelectedIndex(int index)
    {
        if (index <= 0) rectTransform.anchoredPosition  = new Vector3(-cellSize/2f,0,0);
        else rectTransform.anchoredPosition = new Vector3(-cellSize/2f - index*(cellSize + cellSpacing),0,0);
        location = rectTransform.anchoredPosition;
        TriggerIndexChangeEvent();
    }



    private void HandleAddItem(AddInventoryItemData data)
    {
        count+= 1;
        HandleItemsNumberChange();
    }

    private void HandleRemoveItem(RemoveInventoryItemData data)
    {
        count -= 1;
        HandleItemsNumberChange();
    }

    private void HandleItemsNumberChange()
    {
        Initialize();
        if (index >= count) index = Mathf.Max(0,count-1);
        // TriggerIndexChangeEvent();
        SetSelectedIndex(index);
    }

    public void TriggerIndexChangeEvent()
    {
        EventSystems.EventManager.Instance.TriggerEvent<CraftMenuChangeIndexData>(new CraftMenuChangeIndexData(index));
    }

}
