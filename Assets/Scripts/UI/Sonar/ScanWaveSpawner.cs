using UnityEngine;
using Mirror;
using EventSystems;

namespace MD.Character
{
    public class ScanWaveSpawner: NetworkBehaviour
    {
        [SerializeField]
        private GameObject ScanWave = null;

        public override void OnStartServer()
        {
            base.OnStartServer();
            EventManager.Instance.StartListening<ScanWaveSpawnData>(SpawnScanWave);
        }
        [ServerCallback]
        private void OnDisable()
        {
            EventManager.Instance.StopListening<ScanWaveSpawnData>(SpawnScanWave);
        }

        private void SpawnScanWave(ScanWaveSpawnData data)
        {
            GameObject scanWave = Instantiate(ScanWave, data.owner.transform.position, Quaternion.identity);
            NetworkServer.Spawn(scanWave, data.owner.connectionToClient);
        }
    }
}
