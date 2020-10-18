using UnityEngine;

namespace MD.Tutorial
{
    public class SonarTutorialWrapper : MonoBehaviour
    {
        [SerializeField]
        private GameObject genManagerMockup = null;

        [SerializeField]
        private Sonar sonar = null;

        private void Awake()
        {
            sonar.BindScanAreaData(genManagerMockup.GetComponent<IMapManager>());
        }
    }
}