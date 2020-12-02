using UnityEngine;

namespace MD.Map.Core
{
    [RequireComponent(typeof(DiggableData))]
    public class MapTester : MonoBehaviour
    {
        private IDiggableData diggableData;

        void Start()
        {
            diggableData = GetComponent<IDiggableData>();
            Populate();
            TestSetValid();
            TestSetInvalid();
        }

        private void Populate()
        {
            var tiles = new (Vector2Int, ITileData)[]
            {
                (new Vector2Int(0, 0), new TileData(DiggableType.CommonGem)),
                (new Vector2Int(0, 3), new TileData(DiggableType.UncommonGem)),
                (new Vector2Int(5, 6), new TileData(DiggableType.RareGem)),
                (new Vector2Int(-2, 2), TileData.Empty),
                (new Vector2Int(-8, 9), new TileData(DiggableType.CommonGem))
            };

            diggableData.Populate(tiles);
            diggableData.Log();
        }

        private void TestSetValid()
        {
            Debug.Log("---------TEST VALID SET TILE DATA--------------");

            try
            {
                diggableData.SetAt(0, 0, new TileData(DiggableType.UncommonGem));
                diggableData.SetAt(5, 6, new TileData(DiggableType.CommonGem));
                diggableData.Log();
            }
            catch (InvalidTileException)
            {
                Debug.LogError("Invalid tile");
            }
            finally
            {
                Debug.Log("------------------------------------------");
            }
        }

        private void TestSetInvalid()
        {
            Debug.Log("---------TEST INVALID SET TILE DATA--------------");

            try
            {
                diggableData.SetAt(0, 1, new TileData(DiggableType.RareGem));
            }
            catch (InvalidTileException)
            {
                Debug.LogError("Invalid tile");
            }
            finally
            {
                Debug.Log("------------------------------------------");
            }
        }
    }
}