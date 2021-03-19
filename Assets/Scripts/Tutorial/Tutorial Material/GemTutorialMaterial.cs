using UnityEngine;

namespace MD.Tutorial
{
    public class GemTutorialMaterial : TutorialMaterial<DiggableVisibleData, DiggableContactData, DigInvokeData>
    {
        [SerializeField]
        private GameObject storage = null;

        protected override void HandleEventT2(DigInvokeData _)
        {
            base.HandleEventT2(_);
            storage.SetActive(true);
        }
    }
}
