using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MD.Tutorial
{
    public class TutorialMockup : MonoBehaviour, IMapManager
    {
        [SerializeField]
        private GameObject commonGemMockup = null;

        [SerializeField]
        private Transform player = null;

        [SerializeField]
        private float offsetX = 3f, offsetY = 0f;

        private bool setGemPos = false;

        public ScanAreaData GetScanAreaData(Vector2[] posToScan) => new ScanAreaData(GenTileData(posToScan).ToArray());

        private IEnumerable<ScanTileData> GenTileData(Vector2[] posToScan)
        {
            for (int i = 0; i < posToScan.Length; i++)
            {
                
                if (!setGemPos && commonGemMockup != null &&
                posToScan[i].x.IsEqual((player.position.x + .5f).Round() + offsetX) &&
                posToScan[i].y.IsEqual((player.position.y + .5f).Round() + offsetY))
                {
                    Debug.Log((player.position.x).Round() + " " + (player.position.y).Round());
                    commonGemMockup.transform.position = new Vector3(
                        (player.position.x + .5f).Round() + offsetX,
                        (player.position.y + .5f).Round() + offsetY,
                        0f);
                    setGemPos = true;
                }

                if (commonGemMockup != null &&
                    posToScan[i].x.IsEqual(commonGemMockup.transform.position.x) &&
                    posToScan[i].y.IsEqual(commonGemMockup.transform.position.y))
                    yield return new ScanTileData(posToScan[i], 1);

                yield return new ScanTileData(posToScan[i], 0);
            }
        }

        public Vector2Int GetMapSize()
        {
            throw new System.NotImplementedException();
        }

        public Vector2Int PositionToIndex(Vector2 position)
        {
            throw new System.NotImplementedException();
        }

        public bool TrySpawnDiggableAtIndex(Vector2Int idx, DiggableType diggable, GameObject prefab)
        {
            throw new System.NotImplementedException();
        }
    }
}