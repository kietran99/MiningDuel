using UnityEngine;

namespace MD.General
{
    [CreateAssetMenu(fileName = "Global Settings", menuName = "Generator/General/Global Settings")]
    public class GlobalSettings : ScriptableObject
    {
        [SerializeField]
        private bool shouldShowFPS = false;

        public bool ShouldShowFPS
        {
            get => shouldShowFPS;
            set => shouldShowFPS = value;
        }
    }
}
