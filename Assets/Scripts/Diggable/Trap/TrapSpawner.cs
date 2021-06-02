using UnityEngine;
using Mirror;

namespace MD.Diggable.Projectile
{
    public class TrapSpawner : NetworkBehaviour
    {
        [SerializeField]
        private GameObject LinkedTrapPrefab = null;

        [SerializeField]
        private GameObject CamoPersePrefab = null;

        public override void OnStartServer()
        {
            base.OnStartServer();
            EventSystems.EventConsumer.GetOrAttach(gameObject).StartListening<LinkedTrapSpawnData>(SpawnLinkedTrap);
            EventSystems.EventConsumer.GetOrAttach(gameObject).StartListening<CamoPerseSpawnData>(SpawnCamoTrap);
        }

        private void SpawnLinkedTrap(LinkedTrapSpawnData data)
        {
            var obj = Instantiate(LinkedTrapPrefab, new Vector3(data.x,data.y,0),Quaternion.identity);
            NetworkServer.Spawn(obj,data.owner.connectionToClient);
            obj.GetComponent<LinkedTrap>().RpcAssignOwnerAndLinkTraps(data.owner);
        }

        private void SpawnCamoTrap(CamoPerseSpawnData data)
        {
            var obj = Instantiate(CamoPersePrefab, new Vector3(data.x,data.y,0),Quaternion.identity);
            NetworkServer.Spawn(obj,data.owner.connectionToClient);
            obj.GetComponent<MD.Quirk.CamoPerse>().RpcSetUp(data.owner);
        }
    }
}
