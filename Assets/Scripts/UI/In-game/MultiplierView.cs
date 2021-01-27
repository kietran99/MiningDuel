using MD.Character;
using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    public class MultiplierView : MonoBehaviour
    {
        [SerializeField]
        private Text multiplierText = null;

        private const int MAX_DIGIT = 3;
        private const string START_MULT_STR = "x1";
        private System.Text.StringBuilder multBuilder = new System.Text.StringBuilder(START_MULT_STR, MAX_DIGIT + 1);

        void Start()
        {
            multiplierText.text = MakeMultiplier(1);
            EventSystems.EventManager.Instance.StartListening<MultiplierChangeData>(HandleMultiplierChange);            
        }

        void OnDestroy()
        {
            EventSystems.EventManager.Instance.StopListening<MultiplierChangeData>(HandleMultiplierChange);            
        }

        private void HandleMultiplierChange(MultiplierChangeData multiplierChangeData)
        {
            multiplierText.text = MakeMultiplier(multiplierChangeData.newMult);
        }

        private string MakeMultiplier(float mult)
        {
            multBuilder.Remove(1, multBuilder.Length - 1);
            multBuilder.Append(mult);
            return multBuilder.ToString();
        }
    }
}
