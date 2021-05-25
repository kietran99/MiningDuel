public struct GemStackUsedData: EventSystems.IEventData
{
    //index of the first used gem start from bottom of stack
    public int pos;
    //number of gems used
    public int length;
    public GemStackUsedData(int pos, int length)
    {
        this.pos = pos;
        this.length = length;
    }
}