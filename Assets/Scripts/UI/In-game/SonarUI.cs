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

    private WaitForSeconds synctimeWait;

    void Start()
    {
        EventSystems.EventManager.Instance.StartListening<ScanWaveChangeData>(UpdateUI);
        barMaskUnitWidth = BarMask.sizeDelta.x/30f;
        // StartCoroutine(nameof(SimulateRePlenish));
        barMaskMaxWidth = BarMask.sizeDelta.x;
        barMaskSizeDelta = BarMask.sizeDelta;
        synctimeWait = new WaitForSeconds(syncTime/10f);
    }
    void OnDestroy()
    {
        EventSystems.EventManager.Instance.StopListening<ScanWaveChangeData>(UpdateUI);
    }

    // Update is called once per frame
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
        float delta = targetLevel*barMaskUnitWidth-barMaskSizeDelta.x;
        float deltaUnit = delta/10f;
        int currentTargetLevel = targetLevel;
        for (int i= 0; i<10; i++)
        {
            barMaskSizeDelta.x = barMaskSizeDelta.x + deltaUnit;
            BarMask.sizeDelta = barMaskSizeDelta;
            yield return synctimeWait;
        }
        if (currentTargetLevel != targetLevel)
        {
            StartCoroutine(nameof(SimulateChange));
        }
        isSimulating = false;
    }
}
