using System;
using UnityEngine;

namespace MD.Tutorial
{
    public class GemTutorialMaterial : TutorialMaterial<DiggableVisibleData, DiggableContactData, DigInvokeData, Character.FinalScoreChangeData>
    {
        [SerializeField]
        private GameObject storage = null;

        protected override void Start()
        {
            base.Start();
            GetComponent<EventSystems.EventConsumer>().StartListening<TutorialStateChangeData>(MaySpawnStorage);
        }

        private void MaySpawnStorage(TutorialStateChangeData obj)
        {
            // Debug.Log(obj.index);
            if (obj.lineIdx == triggerLineIndices[3])
            {
                storage.SetActive(true);
            }
        }
    }
}
