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

    private bool isDraging;

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
        transform.position = location - new Vector3(difference,0,0);
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
            StartCoroutine(SmoothMove(transform.position,newLocation));
            location = newLocation;
        }
        else
        {
            StartCoroutine(SmoothMove(transform.position, location));
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
            transform.position = Vector3.Slerp(start,end,elapsedTime*SwipeSpeed);
            yield return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        location = transform.position;
        GridLayoutGroup glg = GetComponent<GridLayoutGroup>();
        swipeLength = glg.cellSize.x + glg.spacing.x;
        cellSize = glg.cellSize.x;
        index =0;
    }

}
