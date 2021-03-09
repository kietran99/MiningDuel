using Functional.Type;
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

        [SerializeField]
        private GameObject materialPrefab = null;

        [TextArea]
        [SerializeField]
        private string[] lines = null;

        [SerializeField]
        private UIFocus[] focusLines = null;

        public Option<GameObject> MaterialPrefab => materialPrefab != null ? materialPrefab : Option<GameObject>.None;

        public string[] Lines => lines;

        public UIFocus[] FocusLines => focusLines;
    }
}