using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanControl: MonoBehaviour
{
    public void Scan()
    {
        EventSystems.EventManager.Instance.TriggerEvent(new ScanInvokeData());
    }
}
