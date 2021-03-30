using UnityEngine;

namespace MD.Tutorial
{
    public class TutorialWaveSpawner : MonoBehaviour
    {
            [SerializeField]
            private GameObject ScanWave = null;

            private Transform player;

            private void Start()
            {
                player = GameObject.FindGameObjectWithTag(Constants.PLAYER_TAG).transform;
                gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<ScanInvokeData>(SpawnScanWave);
            }

            private void SpawnScanWave(ScanInvokeData data)
            {
                GameObject scanWave = Instantiate(ScanWave, player.position, Quaternion.identity);
            }
    }
}
