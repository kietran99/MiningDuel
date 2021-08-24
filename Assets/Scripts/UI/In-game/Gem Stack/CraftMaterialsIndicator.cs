using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftMaterialsIndicator : MonoBehaviour
{
    MD.CraftingSystem.GemStackManager gemStack = null;
    [SerializeField]
    private float slotWidth = 56f;

    private Vector2 baseSize = Vector2.zero;

    private float basePadding  = 0f;

    RectTransform rectTransform = null;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        baseSize = rectTransform.sizeDelta;
        basePadding = rectTransform.anchoredPosition.x;
        gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<CraftMenuChangeIndexData>(HandleChangeIndex);
        ServiceLocator.Resolve<MD.CraftingSystem.GemStackManager>(out gemStack);
        rectTransform.sizeDelta = Vector2.zero;
    }

    void HandleChangeIndex(CraftMenuChangeIndexData data)
    {
        (int, int) info= gemStack.GetCraftItemMaterialsInfor(data.index);
        SetPosition(info.Item1,info.Item2);
    }

    // Update is called once per frame
    void SetPosition(int index, int length)
    {
        // Debug.Log("set item index" + index + " length" + length);
        float offsetWidth = slotWidth*index;
        float width = length*slotWidth;
        rectTransform.anchoredPosition = new Vector2(basePadding + offsetWidth,rectTransform.anchoredPosition.y);
        rectTransform.sizeDelta = new Vector2(width,baseSize.y);
    }
}
