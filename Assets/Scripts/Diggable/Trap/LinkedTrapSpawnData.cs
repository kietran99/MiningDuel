using Mirror;
public struct LinkedTrapSpawnData : EventSystems.IEventData
{
    public NetworkIdentity owner;
    public int x;
    public int y;

    public LinkedTrapSpawnData(NetworkIdentity owner, int x, int y)
    {
        this.owner = owner;
        this.x = x;
        this.y = y;
    }

}
