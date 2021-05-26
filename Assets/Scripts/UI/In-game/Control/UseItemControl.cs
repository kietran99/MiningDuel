using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UseItemControl: MonoBehaviour
{
    [SerializeField]
    private GameObject Button;

    void Start()
    {
        gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<MD.UI.SetCraftButtonData>(HandleSetButtonData);
    }

    public void HandleSetButtonData(MD.UI.SetCraftButtonData data)
    {
        if (data.status)
        {
            if (!Button.activeInHierarchy)
            {
                Button.SetActive(true);
            }
        }
        else
        {
            if (Button.activeInHierarchy)
            {
                Button.SetActive(false);
            }
        }
    }

    public void UseItem()
    {
        EventSystems.EventManager.Instance.TriggerEvent(new UseItemInvokeData());
    }
}
