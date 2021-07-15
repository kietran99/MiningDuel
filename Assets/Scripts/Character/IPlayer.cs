using Mirror;

public interface IPlayer //interface for players and bots 
{
    NetworkIdentity GetNetworkIdentity();
    int GetUID();
}