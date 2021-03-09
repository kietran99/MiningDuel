using System.Collections.Generic;
using Functional.Type;
using UnityEngine;

namespace MD.Tutorial
{
    public class TutorialState
    {
        private string[] lines;
        private Option<Dictionary<int, string>> maybeFocusDict;
        private Option<int[]> maskToggleIndices;
        private List<int> impassableLineIndices;

        private int curLineIdx;
        
        private string CurLine => lines[curLineIdx];

        public TutorialState(string[] lines, Option<Dictionary<int, string>> maybeFocusDict, Option<int[]> maskToggleIndices, List<int> impassableLineIndices)
        {
            this.lines = lines;
            this.maybeFocusDict = maybeFocusDict;
            this.maskToggleIndices = maskToggleIndices;
            this.impassableLineIndices = impassableLineIndices;
            curLineIdx = 0;
            EventSystems.EventManager.Instance.TriggerEvent(new TutorialStateChangeData(curLineIdx, CurLine, lines.Length == 1, false, TryGetFocusObjName(curLineIdx)));
        }

        private Option<string> TryGetFocusObjName(int idx)
        {
            return maybeFocusDict
                .Match(
                    dict => 
                    {
                        if (dict.TryGetValue(idx, out string objName))
                        {
                            return objName;
                        }

                        return Option<string>.None;
                    }
                    , () => Option<string>.None
                );
        }
    
        public void RequestNextState()
        {
            if (curLineIdx >= lines.Length - 1)
            {
                Debug.Log("Last line is reached");
                return;
            }

            if (ShouldWaitForTrigger)
            {
                Debug.Log("Follow the instructions to proceed with the tutorial");
                return;
            }

            var nextLineIdx = ++curLineIdx;
            EventSystems.EventManager.Instance.TriggerEvent(
                new TutorialStateChangeData(
                    curLineIdx,
                    CurLine, 
                    nextLineIdx == (lines.Length - 1), 
                    maskToggleIndices.Match(indices => indices.Find(idx => idx == curLineIdx).HasValue, () => false),
                    TryGetFocusObjName(nextLineIdx))
            );

            if (ShouldWaitForTrigger)
            {
                EventSystems.EventManager.Instance.StartListening<TutorialTriggerData>(HandleTrigger);
            }
        }

        private bool ShouldWaitForTrigger => impassableLineIndices.Contains(curLineIdx);  

        private void HandleTrigger(TutorialTriggerData triggerData)
        {
            if (triggerData.index != curLineIdx)
            {
                return;
            }

            EventSystems.EventManager.Instance.StopListening<TutorialTriggerData>(HandleTrigger);
            impassableLineIndices.Remove(triggerData.index);
            RequestNextState();
        }
    }
}