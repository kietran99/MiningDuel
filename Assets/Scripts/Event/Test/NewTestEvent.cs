public struct NewTestEvent : EventSystems.IEventData
{
    public int myInt;
    public float myFloat;

    public NewTestEvent(int myInt, float myFloat)
    {
        this.myInt = myInt;
        this.myFloat = myFloat;
    }
}