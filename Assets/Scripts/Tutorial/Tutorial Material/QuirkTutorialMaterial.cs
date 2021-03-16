using System;
using MD.UI;
using UnityEngine;

namespace MD.Tutorial
{
    public class QuirkTutorialMaterial : TutorialMaterial<UI.QuirkInvokeData>
    {
        [SerializeField]
        private int obtainQuirkIndex = 0;

        [SerializeField]
        private Quirk.QuirkData quirkData = null;

        protected override void Start()
        {
            base.Start();
            GetComponent<EventSystems.EventConsumer>().StartListening<TutorialStateChangeData>(HandleTutorialStateChange);
        }

        private void HandleTutorialStateChange(TutorialStateChangeData stateChangeData)
        {
            if (stateChangeData.lineIdx != obtainQuirkIndex)
            {
                return;
            }

            EventSystems.EventManager.Instance.TriggerEvent(new Quirk.QuirkObtainData(quirkData.ObtainSprite, quirkData.Description));
        }
    }
}
