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
            GetComponent<IDiggableGenerator>().ProjectileObtainEvent += HandleProjetileObtain;
        }

        public override void OnStopServer()
        {
            GetComponent<IDiggableGenerator>().ProjectileObtainEvent -= HandleProjetileObtain;
        }
        [Server]
        private void HandleProjetileObtain(NetworkConnection diggerConn, ProjectileObtainData projObtainData)
        {
            if (projObtainData.type.Equals(DiggableType.LINKED_TRAP))
            {
                var inventory =  diggerConn.identity.gameObject.GetComponent<InventoryController>();
                if (inventory == null)
                {
                    Debug.LogError(diggerConn.identity.gameObject +" has no inventory controller component");
                    return;
                }
                inventory.TargetObtainTraps(diggerConn);
            }
            else
            {
                Spawn(diggerConn, projObtainData);
            }
        }

        [TargetRpc]
        private void TargetAddTrap(NetworkConnection diggercon)
        {

        }

        [Server]
        private void Spawn(NetworkConnection diggerConn, ProjectileObtainData projObtainData)
        {
            if (projObtainData.type.Equals(DiggableType.LINKED_TRAP))
            {

                return;
            }

            var digger = projObtainData.thrower;
            var holdingProjectile = Instantiate(exposedBombPrefab, digger.gameObject.transform);
            holdingProjectile.transform.position = new Vector3(0, 1f, 0);
            holdingProjectile.SetThrower(digger);
            digger.GetComponent<MD.Character.ThrowAction>().SetHoldingProjectile(holdingProjectile);
            NetworkServer.Spawn(holdingProjectile.gameObject);
        }
    }
}