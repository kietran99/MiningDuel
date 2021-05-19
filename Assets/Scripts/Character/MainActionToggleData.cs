namespace MD.Character
{
    public class MainActionToggleData : EventSystems.IEventData
    {
        public MainActionType actionType;

        public MainActionToggleData(MainActionType actionType)
        {
            this.actionType = actionType;
        }
    }
}