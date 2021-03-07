using System.Collections.Generic;
using Functional.Type;
using UnityEngine;

namespace MD.Tutorial
{
    [CreateAssetMenu(fileName="Tutorial Data", menuName="Generator/Tutorial/Data")]
    public class TutorialNavigateData : ScriptableObject
    {
        [System.Serializable]
        public struct UIFocus
        {
            public string objectName;
            public int lineIdx;
        }

        #region SERIALIZE FIELDS
        [SerializeField]
        private AbstractTutorialMaterial tutorialMaterial = null;

        [TextArea]
        [SerializeField]
        private string[] lines = null;

        [SerializeField]
        private UIFocus[] focusLines = null;
        #endregion

        private Option<AbstractTutorialMaterial> TutorialMaterial => tutorialMaterial != null ? tutorialMaterial : Option<AbstractTutorialMaterial>.None;

        public TutorialState SetupEnvironment() 
        {
            TutorialMaterial.Match(prefab => Instantiate(prefab), () => Debug.Log("No material prefab was found"));
            // tutorialMaterial.TriggerLineIndices.ForEach(_ => Debug.Log(_));
            return new TutorialState(lines, MakeMaybeFocusDict(focusLines), tutorialMaterial.TriggerLineIndices);
        }
    
        private Option<Dictionary<int, string>> MakeMaybeFocusDict(UIFocus[] focusLines)
        {
            if (focusLines == null || focusLines.Length == 0)
            {
                return Option<Dictionary<int, string>>.None;
            }

            Dictionary<int, string> focusDict = new Dictionary<int, string>();
            foreach (var entry in focusLines)
            {
                focusDict.Add(entry.lineIdx, entry.objectName);
            }

            return focusDict;
        }
    }
}