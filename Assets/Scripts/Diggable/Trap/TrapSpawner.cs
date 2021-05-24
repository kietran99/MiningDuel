using UnityEngine;
using Mirror;

namespace MD.Diggable.Projectile
{
    public class TrapSpawner : NetworkBehaviour
    {
        [SerializeField]
        private GameObject LinkedTrapPrefab = null;

        public override void OnStartServer()
        {
            base.OnStartServer();
            EventSystems.EventManager.Instance.StartListening<LinkedTrapSpawnData>(SpawnLinkedTrap);
        }

        void OnDisable()
        {
            if (isServer)
            {
                EventSystems.EventManager.Instance.StopListening<LinkedTrapSpawnData>(SpawnLinkedTrap);
            }
        }

        private void SpawnLinkedTrap(LinkedTrapSpawnData data)
        {
            var obj = Instantiate(LinkedTrapPrefab, new Vector3(data.x,data.y,0),Quaternion.identity);
            NetworkServer.Spawn(obj,data.owner.connectionToClient);
            obj.GetComponent<LinkedTrap>().RpcAssignOwnerAndLinkTraps(data.owner);
        }
    }
}
