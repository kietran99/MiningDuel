using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CraftingMenuDrag : MonoBehaviour, IDragHandler, IEndDragHandler
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

    private int count = 5;

    private float swipeLength;

    private Vector3 location;
    private float cellSize;
    private float cellSpacing;

    private bool isDraging;
    private RectTransform rectTransform;

    public void OnDrag(PointerEventData data)
    {
        float difference = (data.pressPosition.x - data.position.x)*dragSpeed;
        if ((index == 0 && difference < 0) ||  (index == count-1 && difference > 0 ))
        {
            difference =  Mathf.Clamp(difference,-dragMaxExceedLength,dragMaxExceedLength);
        }
        else
        {
            difference =  Mathf.Clamp(difference,-swipeLength -dragMaxExceedLength, swipeLength + dragMaxExceedLength);
        }
        transform.localPosition = location - new Vector3(difference,0,0);
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
            }
            else if (percentage < 0 && index > 0)
            {
                newLocation += new Vector3(swipeLength,0,0);
                index--;
            }
            StartCoroutine(SmoothMove(transform.localPosition,newLocation));
            location = newLocation;
        }
        else
        {
            StartCoroutine(SmoothMove(transform.localPosition, location));
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
            transform.localPosition = Vector3.Slerp(start,end,elapsedTime*SwipeSpeed);
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

        var eventConsumer = GetComponent<EventSystems.EventConsumer>();
        eventConsumer.StartListening<CraftItemsNumberChangeData>(HandleItemsNumberChange);   
    }

    private void Initialize()
    {
        rectTransform.sizeDelta = new Vector2(count*cellSize + (count-1)*cellSpacing,rectTransform.sizeDelta.y);
    }

    private void SetSelectedIndex(int index)
    {
        if (index <= 0) transform.localPosition = new Vector3(-cellSize/2f,0,0);
        else transform.localPosition = new Vector3(-cellSize/2f + index*cellSize + (index-1)*cellSpacing,0,0);
        location = transform.localPosition;
    }

    private void HandleItemsNumberChange(CraftItemsNumberChangeData data)
    {
        count = data.numOfItems;
        Initialize();
        if (index >= count) index = count -1;
        SetSelectedIndex(index);
    }

}
