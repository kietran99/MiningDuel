using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    public class SonarUI : MonoBehaviour
    {
        [SerializeField]
        private RawImage fillArea = null;

        [SerializeField]
        private RectTransform barMask = null;

        [SerializeField]
        float syncTime = .5f;

        [SerializeField]
        private int targetLevel = 0;

        [SerializeField]
        private Gradient batteryColor = null;
        
        private float barMaskUnitWidth, barMaskMaxWidth;

        private float speedDivide = 1.9f;
        
        private bool isSimulating;

        private Vector2 barMaskSizeDelta;

        // private WaitForSeconds synctimeWait;

        void Start()
        {
            EventSystems.EventConsumer.Attach(gameObject).StartListening<ScanWaveChangeData>(UpdateUI);
            barMaskUnitWidth = barMask.sizeDelta.x / 30f;
            barMaskMaxWidth = barMask.sizeDelta.x;
            barMaskSizeDelta = barMask.sizeDelta;
        }

        void Update()
        {
            Rect fillAreaRect = fillArea.uvRect;
            fillAreaRect.x += .1f * Time.deltaTime;

            if (fillAreaRect.x > 100f) 
            {
                fillAreaRect.x = 0;
            }

            fillArea.uvRect = fillAreaRect;

            if (!isSimulating)
            {
                barMaskSizeDelta.x = barMaskSizeDelta.x + speedDivide * Time.deltaTime * barMaskUnitWidth;

                if (barMaskSizeDelta.x > barMaskMaxWidth) 
                {
                    barMaskSizeDelta.x = barMaskMaxWidth;
                }

                barMask.sizeDelta = barMaskSizeDelta;

                fillArea.color = batteryColor.Evaluate(barMaskSizeDelta.x / barMaskMaxWidth);
            }       
        }

        void UpdateUI(ScanWaveChangeData data)
        {
            targetLevel = data.currentLevel;

            if (targetLevel == Mathf.FloorToInt(barMaskSizeDelta.x / barMaskUnitWidth)) 
            {
                return;
            }

            if (!isSimulating) 
            {
                StartCoroutine(nameof(SimulateChange));
            }
        }

        private IEnumerator SimulateChange()
        {
            isSimulating = true;
            float targetWidth = targetLevel * barMaskUnitWidth;
            float delta = targetWidth - barMaskSizeDelta.x; 
            int currentTargetLevel = targetLevel;
            float speed = delta / syncTime;
            float elapsedTime = 0f;

            while (elapsedTime < syncTime)
            {
                barMaskSizeDelta.x = barMaskSizeDelta.x + speed * Time.deltaTime;
                if ((delta >= 0 && barMaskSizeDelta.x >= targetWidth) 
                || (delta < 0 && barMaskSizeDelta.x <= targetWidth))
                {
                    barMaskSizeDelta.x = targetWidth;
                    barMask.sizeDelta = barMaskSizeDelta;

                    fillArea.color = batteryColor.Evaluate(barMaskSizeDelta.x / barMaskMaxWidth);
                    break;
                }

                barMask.sizeDelta = barMaskSizeDelta;
                elapsedTime += Time.deltaTime;

                fillArea.color = batteryColor.Evaluate(barMaskSizeDelta.x / barMaskMaxWidth);
                yield return null;
            }

            if (currentTargetLevel != targetLevel)
            {
                StartCoroutine(nameof(SimulateChange));
            }

            isSimulating = false;
        }
    }
}
