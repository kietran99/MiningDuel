public class RoomWindowToggleData : EventSystems.IEventData
{
    public bool isActive;

    public RoomWindowToggleData(bool isActive)
    {
        this.isActive = isActive;
    }
}
