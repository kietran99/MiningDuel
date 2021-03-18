using MD.Diggable.Gem;
using UnityEngine;

namespace MD.Tutorial
{
    [RequireComponent(typeof(GemValue))]
    public class MightyBlessingObtain : MonoBehaviour
    {
        private static int finalScore = 0;

        private GemValue gemValue;

        private void Start()
        {
            gemValue = GetComponent<GemValue>();
            gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<UI.QuirkInvokeData>(MightyBlessingDig);
        }

        public void MightyBlessingDig(UI.QuirkInvokeData _)
        {
            finalScore += gemValue.Value;
            EventSystems.EventManager.Instance.TriggerEvent(new Character.FinalScoreChangeData(finalScore));
            gameObject.SetActive(false);
        }
    }
}
