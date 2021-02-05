using UnityEngine;
using Mirror;
using MD.Diggable.Projectile;

namespace MD.Diggable.Core
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
        private void Spawn(NetworkConnection diggerConn, ProjectileObtainData projObtainData)
        {
            var digger = projObtainData.thrower;
            var holdingProjectile = Instantiate(exposedBombPrefab, digger.gameObject.transform);
            holdingProjectile.transform.position = new Vector3(0, 1f, 0);
            holdingProjectile.SetThrower(digger);
            digger.GetComponent<MD.Character.ThrowAction>().SetHoldingProjectile(holdingProjectile);
            NetworkServer.Spawn(holdingProjectile.gameObject);
        }
    }
}