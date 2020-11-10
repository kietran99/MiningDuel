using UnityEngine;
using Mirror;
using MD.Diggable.Projectile;
public class PlayerItemSpawner: NetworkBehaviour
{
    [SerializeField]
    private ProjectileLauncher exposedBombPrefab = null;

    [Server]
    public void SpawnBombAtPlayer(NetworkIdentity player)
    {
        var bombInstance = Instantiate(exposedBombPrefab, player.gameObject.transform);
        //set postion on server
        transform.parent = player.transform;
        transform.position = new Vector3(0,1f,0);
        //on client
        bombInstance.SetOwner(player);
        player.GetComponent<MD.Character.ThrowAction>().SetHoldingProjectile(bombInstance);
        NetworkServer.Spawn(bombInstance.gameObject);
    }
}