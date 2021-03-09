using Functional.Type;

namespace MD.Tutorial
{
    public struct TutorialStateChangeData : EventSystems.IEventData
    {
        public string line;
        public bool isLastLine, shouldToggleMask;
        public Functional.Type.Option<string> maybefocusObjectName;

        public TutorialStateChangeData(string line, bool isLastLine, bool shouldToggleMask, Option<string> maybefocusObjectName)
        {
            this.line = line;
            this.isLastLine = isLastLine;
            this.shouldToggleMask = shouldToggleMask;
            this.maybefocusObjectName = maybefocusObjectName;
        }
    }
}