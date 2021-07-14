namespace MD.UI
{
    public struct SetCraftButtonData: EventSystems.IEventData
    {
        public bool status;
        public SetCraftButtonData(bool status) => this.status = status;
    }
}