
public struct NewGemData: EventSystems.IEventData
{
    public float x,y;
    public int type;

    public NewGemData(float x,float y,int type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
    }
}
