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
                    posToScan[i].x.IsEqual(Mathf.Floor(player.position.x) + offsetX) &&
                    posToScan[i].y.IsEqual(Mathf.Floor(player.position.y) + offsetY))
                {
                    commonGemMockup.transform.position = new Vector3(
                        Mathf.Floor(player.position.x) + offsetX,
                        Mathf.Floor(player.position.y) + offsetY, 0f);
                    setGemPos = true;
                }

                if (commonGemMockup != null &&
                    posToScan[i].x.IsEqual(commonGemMockup.transform.position.x) &&
                    posToScan[i].y.IsEqual(commonGemMockup.transform.position.y))
                    yield return new ScanTileData(posToScan[i], 1);

                yield return new ScanTileData(posToScan[i], 0);
            }
        }
    }
}