public struct NewTestEvent : EventSystems.IEventData
{
    public int x;

    public NewTestEvent(int x) => this.x = x;
}