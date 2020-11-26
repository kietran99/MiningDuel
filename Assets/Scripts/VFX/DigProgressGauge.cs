using UnityEngine;
using MD.Diggable.Gem;
using MD.UI;
using Utils;

namespace MD.VisualEffects
{
    public class DigProgressGauge : MonoBehaviour
    {
        [SerializeField]
        private GameObject gaugeContainer = null;

        [SerializeField]
        private Transform fillArea = null;
        
        private FlowMux<DigProgressData> digProgressMux = new FlowMux<DigProgressData>();

        private void Start() 
        {      
            AddFlows();                  
            EventSystems.EventManager.Instance.StartListening<DigProgressData>(ResolveProgressInput);
            EventSystems.EventManager.Instance.StartListening<JoystickDragData>(Hide);
        }
       
        private void AddFlows()
        {
            digProgressMux.AddShape(
            new FlowShape<DigProgressData>(data => data.current > data.max, _ => Debug.LogError("Current value must be less than max value")));

            digProgressMux.AddShape(new FlowShape<DigProgressData>(data => data.current == 0, _ => Hide()));

            digProgressMux.AddShape(new FlowShape<DigProgressData>(data => data.current < data.max, data => Fill(data.current, data.max)));

            digProgressMux.AddShape(new FlowShape<DigProgressData>(data => true, _ => Debug.LogError("Unknown dig progress transform")));
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
        // void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.U)) digProgressMux.Resolve(new DigProgressData(4, 10));
        //     else if (Input.GetKeyDown(KeyCode.J)) digProgressMux.Resolve(new DigProgressData(2, 10));
        //     else if (Input.GetKeyDown(KeyCode.I)) digProgressMux.Resolve(new DigProgressData(10, 2));
        //     else if (Input.GetKeyDown(KeyCode.K)) digProgressMux.Resolve(new DigProgressData(0, 10));
        // }

        private void ResolveProgressInput(DigProgressData progressData)
        {
            digProgressMux.Resolve(progressData);
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
