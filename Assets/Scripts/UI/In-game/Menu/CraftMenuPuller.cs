using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CraftMenuPuller : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField]
    RectTransform swipeMenuTransform = null;

    [SerializeField]
    private float SwitchMenuPercentThreshold = .5f;

    [SerializeField]
    private float dragSpeed = .5f;

    SwipeMenu swipeMenu = null;

    private float cellSize = 170f;
    [SerializeField]
    private Vector3 hiddenPosition;
    private float maxDragLength;

    public void OnDrag(PointerEventData data)
    {
        float YDiff= (data.pressPosition.y - data.position.y)*dragSpeed;
        if (YDiff > 0) // move menu to hand curent drag point position; 
        {
            YDiff =  Mathf.Clamp(YDiff, 0 , maxDragLength);
            swipeMenuTransform.anchoredPosition  = hiddenPosition - new Vector3(0,YDiff,0);
        }

    }

    public void OnEndDrag(PointerEventData data)
    {
        float Ypercentage= (data.pressPosition.y - data.position.y)/cellSize;
        Debug.Log(Ypercentage);
        if (Ypercentage > SwitchMenuPercentThreshold) // swipe down
        {
            swipeMenu.SwitchMenu();
        }
        else
        {
            swipeMenu.ReturnToCurrentPostion();
        }
        return;
    }

    private void Start()
    {
        swipeMenu = swipeMenuTransform.GetComponent<SwipeMenu>();
        hiddenPosition = swipeMenu.GetHiddentMenuLocation();
        maxDragLength = cellSize*.7f;
    }
}
