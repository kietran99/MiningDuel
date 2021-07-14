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
            var eventConsumer = EventSystems.EventConsumer.GetOrAttach(gameObject);
            eventConsumer.StartListening<LinkedTrapSpawnData>(SpawnLinkedTrap);
            eventConsumer.StartListening<CamoPerseSpawnData>(SpawnCamoTrap);
        }

        private void SpawnLinkedTrap(LinkedTrapSpawnData data)
        {
            var obj = Instantiate(LinkedTrapPrefab, new Vector3(data.x, data.y, 0f),Quaternion.identity);
            NetworkServer.Spawn(obj,data.owner.connectionToClient);
            obj.GetComponent<LinkedTrap>().RpcAssignOwnerAndLinkTraps(data.owner);
        }

        private void SpawnCamoTrap(CamoPerseSpawnData data)
        {            
            var obj = Instantiate(CamoPersePrefab);
            NetworkServer.Spawn(obj, data.owner.connectionToClient);
            obj.GetComponent<MD.Quirk.CamoPerse>().RpcSetUp(data.owner);
        }
    }
}
