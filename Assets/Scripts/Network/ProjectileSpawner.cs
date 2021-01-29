using UnityEngine;
using Mirror;
using MD.Diggable.Projectile;

public class ProjectileSpawner: NetworkBehaviour
{
    [SerializeField]
    private ProjectileLauncher exposedBombPrefab = null;

    private NetworkIdentity player;

    public override void OnStartAuthority()
    {
        EventSystems.EventManager.Instance.StartListening<ProjectileObtainData>(HandleProjectileObtainEvent);
        player = netIdentity;
    }

    public override void OnStopAuthority()
    {
        EventSystems.EventManager.Instance.StopListening<ProjectileObtainData>(HandleProjectileObtainEvent);
    }

    private void HandleProjectileObtainEvent(ProjectileObtainData projectileObtainData)
    {
        CmdSpawnBombAtPlayer(projectileObtainData.networkIdentity);
    }

    [Command]
    public void CmdSpawnBombAtPlayer(NetworkIdentity player)
    {
        var holdingProjectile = Instantiate(exposedBombPrefab, player.gameObject.transform);
        holdingProjectile.transform.parent = player.transform;
        holdingProjectile.transform.position = new Vector3(0, 1f, 0);
        holdingProjectile.SetThrower(player);
        player.GetComponent<MD.Character.ThrowAction>().SetHoldingProjectile(holdingProjectile);
        NetworkServer.Spawn(holdingProjectile.gameObject);
    }

    [Server]
    public void SpawnBombAtPlayer(NetworkIdentity player)
    {
        var bombInstance = Instantiate(exposedBombPrefab, player.gameObject.transform);
        //set postion on server
        bombInstance.transform.parent = player.transform;
        bombInstance.transform.position = new Vector3(0,1f,0);
        //on client
        bombInstance.SetThrower(player);
        player.GetComponent<MD.Character.ThrowAction>().SetHoldingProjectile(bombInstance);
        NetworkServer.Spawn(bombInstance.gameObject);
    }
}