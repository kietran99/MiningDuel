using Mirror;
public class StoreFinishedData : EventSystems.IEventData
{
    public NetworkIdentity player;
    public StoreFinishedData(NetworkIdentity player) => this.player = player;
}