using Mirror;
public class ScanWaveSpawnData : EventSystems.IEventData
{
    public NetworkIdentity owner;
    public ScanWaveSpawnData(NetworkIdentity owner)
    {
        this.owner = owner;
    }
}