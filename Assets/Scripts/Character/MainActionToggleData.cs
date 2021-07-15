namespace MD.Character
{
    public struct MainActionToggleData : EventSystems.IEventData
    {
        public MainActionType actionType;

        public MainActionToggleData(MainActionType actionType)
        {
            this.actionType = actionType;
        }
    }
}