namespace MD.Tutorial
{
    public struct TutorialStateChangeData : EventSystems.IEventData
    {
        public string line;
        public bool isLastLine;
        public Functional.Type.Option<string> maybefocusObjectName;

        public TutorialStateChangeData(string line, bool isLastLine, Functional.Type.Option<string> maybeFocusObjectName)
        {
            this.line = line;
            this.isLastLine = isLastLine;
            this.maybefocusObjectName = maybeFocusObjectName;
        }
    }
}