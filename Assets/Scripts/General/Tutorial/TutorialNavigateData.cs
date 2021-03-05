using UnityEngine;

namespace MD.Tutorial
{
    [CreateAssetMenu(fileName="Tutorial Data", menuName="Generator/General/Tutorial Data")]
    public class TutorialNavigateData : ScriptableObject
    {
        [System.Serializable]
        public struct UIFocus
        {
            public string gameObjectName;
            public int lineIdx;
        }

        [TextArea]
        [SerializeField]
        private string[] lines = null;

        [SerializeField]
        private UIFocus[] focusLines = null;

        public string[] Lines => lines;

        public UIFocus[] FocusLines => focusLines;
    }
}