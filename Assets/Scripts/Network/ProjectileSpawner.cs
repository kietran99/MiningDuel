using UnityEngine;
using Mirror;
using MD.Diggable.Projectile;

namespace MD.Map.Core
{
    [RequireComponent(typeof(DiggableGenerator))]
    public class ProjectileSpawner: NetworkBehaviour
    {
        [SerializeField]
        private ProjectileLauncher exposedBombPrefab = null;

        public override void OnStartServer()
        {
            GetComponent<IDiggableGenerator>().ProjectileObtainEvent += Spawn;
        }

        public override void OnStopServer()
        {
            GetComponent<IDiggableGenerator>().ProjectileObtainEvent -= Spawn;
        }

        [Server]
        public void Spawn(NetworkConnection conn, ProjectileObtainData projObtainData)
        {
            var player = projObtainData.thrower;
            var holdingProjectile = Instantiate(exposedBombPrefab, player.gameObject.transform);
            holdingProjectile.transform.parent = player.transform;
            holdingProjectile.transform.position = new Vector3(0, 1f, 0);
            holdingProjectile.SetThrower(player);
            player.GetComponent<MD.Character.ThrowAction>().SetHoldingProjectile(holdingProjectile);
            NetworkServer.Spawn(holdingProjectile.gameObject);
        }
    }
}