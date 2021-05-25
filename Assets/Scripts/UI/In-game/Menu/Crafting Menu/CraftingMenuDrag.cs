using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CraftingMenuDrag : MonoBehaviour, IDragHandler, IEndDragHandler
{

    [SerializeField]
    RectTransform swipeMenuTransform = null;

    [SerializeField]
    private float AutoSwitchMenuThreshold = 1f;

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

    private int count = 5;
    SwipeMenu swipeMenu = null;

    private float swipeLength;

    private Vector3 location;
    // private Vector3 YLocation;
    private float cellSize;
    private float cellSpacing;
    private RectTransform rectTransform;
    [SerializeField]
    private Vector3 ShownPosition;
    private float maxDragLength;

    [SerializeField]
    private bool isSwitchMenu = false;
    
    [SerializeField]
    private bool isSwiping = false;


    public void OnDrag(PointerEventData data)
    {

        if (!isSwiping)
        {
            float YDiff= (data.pressPosition.y - data.position.y)*dragSpeed;
            if (YDiff < -.2f*cellSize) // move menu to hand curent drag point position; 
            {
                isSwitchMenu = true;
                YDiff =  Mathf.Clamp(YDiff, -maxDragLength , 0);
                Debug.Log("diff " + YDiff);
                swipeMenuTransform.anchoredPosition  = ShownPosition - new Vector3(0,YDiff,0);
            }
        }

        if (!isSwitchMenu)
        {
            float difference = (data.pressPosition.x - data.position.x)*dragSpeed;
            if (Mathf.Abs(difference) > .1f*cellSize) 
            {
                isSwiping=true;
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
        }

    }

    public void OnEndDrag(PointerEventData data)
    {
        if (isSwitchMenu)
        {
            float Ypercentage= (data.pressPosition.y - data.position.y)/cellSize;;
            if (Ypercentage < AutoSwitchMenuThreshold)
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

        isSwiping = false;
        isSwitchMenu = false;
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
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        GridLayoutGroup glg = GetComponent<GridLayoutGroup>();
        swipeLength = glg.cellSize.x + glg.spacing.x;
        cellSize = glg.cellSize.x;
        cellSpacing = glg.spacing.x;
        index =0;
        Initialize();
        SetSelectedIndex(0);
        var eventConsumer = gameObject.AddComponent<EventSystems.EventConsumer>();
        eventConsumer.StartListening<CraftItemsNumberChangeData>(HandleItemsNumberChange);  

        swipeMenu = swipeMenuTransform.GetComponent<SwipeMenu>();
        ShownPosition= swipeMenu.GetCraftMenuLocation();
        maxDragLength = cellSize*.7f;
    }

    private void Initialize()
    {
        // 2*halfcellsizePadding + num*cell + (num-1)*space
        rectTransform.sizeDelta = new Vector2(cellSize + count*cellSize + (count-1)*cellSpacing,rectTransform.sizeDelta.y);
    }

    private void SetSelectedIndex(int index)
    {
        if (index <= 0) rectTransform.anchoredPosition  = new Vector3(0,0,0);
        else rectTransform.anchoredPosition = new Vector3( -index*(cellSize + cellSpacing), 0, 0);
        location = rectTransform.anchoredPosition;
        TriggerIndexChangeEvent();
    }

    private void HandleItemsNumberChange(CraftItemsNumberChangeData data)
    {
        count = data.numOfItems;
        Initialize();
        if (index >= count) index = Mathf.Max(0,count-1);
        SetSelectedIndex(index);
    }

    public void TriggerIndexChangeEvent()
    {
        EventSystems.EventManager.Instance.TriggerEvent<CraftMenuChangeIndexData>(new CraftMenuChangeIndexData(index));
    }

}
