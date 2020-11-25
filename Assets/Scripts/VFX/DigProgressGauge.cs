using UnityEngine;
using System;
using MD.Diggable.Gem;
using MD.UI;

namespace MD.VisualEffects
{
    public class DigProgressGauge : MonoBehaviour
    {
        private class ProgressTransform
        {
            public ProgressTransform(Func<int, int, bool> canApply, Action<int, int> transform)
            {
                CanApply = canApply;
                Transform = transform;
            }

            public Func<int, int, bool> CanApply { get; }
            public Action<int, int> Transform { get; }           
        }

        [SerializeField]
        private GameObject gaugeContainer = null;

        [SerializeField]
        private Transform fillArea = null;
        
        private ProgressTransform[] progressTransforms;

        private void Start() 
        {            
            progressTransforms = new ProgressTransform[4]
            {
                new ProgressTransform((int cur, int max) => cur > max, (cur, max) => Debug.LogError("Current value must be less than max value")),
                new ProgressTransform((int cur, int max) => cur == 0, (cur, max) => Hide()),
                new ProgressTransform((int cur, int max) => cur < max, Fill),
                new ProgressTransform((int cur, int max) => true, (cur, max) => Debug.LogError("Unknown dig progress transform"))
            };

            EventSystems.EventManager.Instance.StartListening<DigProgressData>(ResolveProgressInput);
            EventSystems.EventManager.Instance.StartListening<JoystickDragData>(Hide);
        }
       
        private void OnDestroy() 
        {
            EventSystems.EventManager.Instance.StopListening<DigProgressData>(ResolveProgressInput);
            EventSystems.EventManager.Instance.StopListening<JoystickDragData>(Hide);
        }

        private void Hide(JoystickDragData dragData)
        {
            if (!dragData.InputDirection.x.IsEqual(0f) && !dragData.InputDirection.y.IsEqual(0f))
            {
                Hide();
            }
        } 

        private void Hide() => gaugeContainer.SetActive(false);

        // For testing purpose only
        // private void Update() 
        // {
        //     if (Input.GetKeyDown(KeyCode.U)) ResolveProgressInput(new DigProgressData(4, 10));
        //     else if (Input.GetKeyDown(KeyCode.J)) ResolveProgressInput(new DigProgressData(2, 10));
        //     else if (Input.GetKeyDown(KeyCode.I)) ResolveProgressInput(new DigProgressData(10, 2));
        //     else if (Input.GetKeyDown(KeyCode.K)) ResolveProgressInput(new DigProgressData(10, 10));
        // }

        private void ResolveProgressInput(DigProgressData progressData)
        {
            //progressHandleDict[(cur, max) => true].Invoke(progressData.current, progressData.max);
            var executor = progressTransforms.LookUp(transform => transform.CanApply(progressData.current, progressData.max)).item;
            executor.Transform(progressData.current, progressData.max);
        }

        private void Fill(int cur, int max)
        {            
            float fraction = (float) cur / (float) max;
            gaugeContainer.SetActive(true);
            //Debug.Log("Fraction to dig successful: " + fraction);
            fillArea.localScale = new Vector3(fraction, fillArea.localScale.y, 1f);
            fillArea.localPosition = new Vector3(-1f + fraction, fillArea.localPosition.y, fillArea.localPosition.z);
        }
    }
}
