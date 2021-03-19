using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SonarUI : MonoBehaviour
{
    [SerializeField]
    RawImage fillArea;
    [SerializeField]
    RectTransform BarMask;

    [SerializeField]
    float syncTime = .5f;
    
    float barMaskUnitWidth;
    float barMaskMaxWidth;

    [SerializeField]
    private int targetLevel;

    private float speedDivide = 1.9f;
    bool isSimulating;

    private Vector2 barMaskSizeDelta;

    // private WaitForSeconds synctimeWait;

    void Start()
    {
        EventSystems.EventManager.Instance.StartListening<ScanWaveChangeData>(UpdateUI);
        barMaskUnitWidth = BarMask.sizeDelta.x/30f;
        barMaskMaxWidth = BarMask.sizeDelta.x;
        barMaskSizeDelta = BarMask.sizeDelta;
    }
    void OnDestroy()
    {
        EventSystems.EventManager.Instance.StopListening<ScanWaveChangeData>(UpdateUI);
    }

    void Update()
    {
        Rect fillAreaRect = fillArea.uvRect;
        fillAreaRect.x += .1f*Time.deltaTime;
        if (fillAreaRect.x >100f) fillAreaRect.x = 0;
        fillArea.uvRect = fillAreaRect;


        if (!isSimulating)
        {
            barMaskSizeDelta.x = barMaskSizeDelta.x + speedDivide*Time.deltaTime*barMaskUnitWidth;
            if(barMaskSizeDelta.x > barMaskMaxWidth) barMaskSizeDelta.x = barMaskMaxWidth;
            BarMask.sizeDelta = barMaskSizeDelta;
        }       
    }

    void UpdateUI(ScanWaveChangeData data)
    {
        targetLevel = data.currentLevel;
        if (targetLevel == Mathf.FloorToInt(barMaskSizeDelta.x/barMaskUnitWidth)) return;
        if (!isSimulating) StartCoroutine(nameof(SimulateChange));
    }

    private IEnumerator SimulateChange()
    {
        isSimulating = true;
        float targetWidth = targetLevel*barMaskUnitWidth;
        float delta = targetWidth-barMaskSizeDelta.x; 
        int currentTargetLevel = targetLevel;
        float speed = delta/syncTime;
        float elapsedTime = 0f;
        while (elapsedTime < syncTime)
        {
            barMaskSizeDelta.x = barMaskSizeDelta.x + speed*Time.deltaTime;
            if ((delta >= 0 && barMaskSizeDelta.x >= targetWidth) 
            || (delta < 0 && barMaskSizeDelta.x <= targetWidth))
            {
                barMaskSizeDelta.x = targetWidth;
                BarMask.sizeDelta = barMaskSizeDelta;
                break;
            }
            BarMask.sizeDelta = barMaskSizeDelta;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (currentTargetLevel != targetLevel)
        {
            StartCoroutine(nameof(SimulateChange));
        }
        isSimulating = false;
    }
}
