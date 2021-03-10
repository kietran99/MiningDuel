using UnityEngine;
using System;

namespace MD.Tutorial
{
    public class TutorialTriggerData : EventSystems.IEventData
    {
        public int index;

        public TutorialTriggerData(int index)
        {
            this.index = index;
        }
    }
}