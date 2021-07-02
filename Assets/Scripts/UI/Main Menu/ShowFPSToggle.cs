using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    public class ShowFPSToggle : MonoBehaviour
    {
        [SerializeField]
        private General.GlobalSettings settings = null;

        [SerializeField]
        private Toggle toggle = null;

        private void OnEnable() => toggle.isOn = settings.ShouldShowFPS;
    }
}
