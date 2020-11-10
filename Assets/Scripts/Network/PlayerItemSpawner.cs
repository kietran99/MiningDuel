using UnityEngine;
using Mirror;
public class PlayerItemSpawner: NetworkBehaviour
{
    [SerializeField]
    private GameObject exposedBombPrefab = null;

    [Server]
    public void SpawnBombAtPlayer(NetworkIdentity player)
    {
        var bombInstance = Instantiate(exposedBombPrefab, player.gameObject.transform);
        NetworkServer.Spawn(bombInstance);
    }
}