using Mirror;
public struct CamoPerseSpawnData : EventSystems.IEventData
{
    public NetworkIdentity owner;
    public int x;
    public int y;

    public CamoPerseSpawnData(NetworkIdentity owner, int x, int y)
    {
        this.owner = owner;
        this.x = x;
        this.y = y;
    }
}