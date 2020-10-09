public struct MoveData : EventSystems.IEventData
{
    public float x, y;

    public MoveData(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}
