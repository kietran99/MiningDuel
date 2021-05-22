using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class SwipeMenu : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;

    [SerializeField]
    float InventoryMenuYPosition;

    [SerializeField]
    float CraftMenuYPosition;

    [SerializeField]
    float moveExceedTime = .1f;

    [SerializeField]
    float moveExceedLength = 10f;

    bool IsInventoryMenu = true;

    private IEnumerator SmoothMove(float YStart, float YEnd)
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
            transform.localPosition = new Vector3(0,Mathf.Lerp(YStart,YExceedEnd,elapsedTime/firstMoveTime));
            yield return null;
        }
        elapsedTime = 0;
        while(elapsedTime < secondMoveTime)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= secondMoveTime) elapsedTime = secondMoveTime;
            // transform.localPosition = Vector3.Slerp(exceedEnd,end,elapsedTime/secondMoveTime);
            transform.localPosition = new Vector3(0,Mathf.Lerp(YExceedEnd,YEnd,elapsedTime/secondMoveTime));
            yield return null;
        }

    }

    public void SwitchMenu()
    {
        if(IsInventoryMenu)
        {
            StartCoroutine(SmoothMove(InventoryMenuYPosition,CraftMenuYPosition));
            IsInventoryMenu = false;
        }
        else
        {
            StartCoroutine(SmoothMove(CraftMenuYPosition,InventoryMenuYPosition));
            IsInventoryMenu = true;
        }
    }
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchMenu();
        }
    }
}
