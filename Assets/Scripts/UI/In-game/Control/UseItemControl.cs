using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UseItemControl: MonoBehaviour
{
    public void UseItem()
    {
        EventSystems.EventManager.Instance.TriggerEvent(new UseItemInvokeData());
    }
}
