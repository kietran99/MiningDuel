using UnityEngine;
using Functional.Type;

namespace MD.Map.Core
{
    [RequireComponent(typeof(DiggableData))]
    public class MapTester : MonoBehaviour
    {
        private IDiggableData diggableData;

        void Start()
        {
            diggableData = GetComponent<IDiggableData>();
            //Populate();
            //TestFormat("TEST VALID SET TILE DATA", TestSetValid);
            //TestFormat("TEST INVALID SET TILE DATA", TestSetInvalid);
            //TestFormat("TEST VALID REDUCE TILE DATA", TestReduceValid);
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
            diggableData.SetAt(0, 0, new TileData(DiggableType.UncommonGem));
            diggableData.SetAt(-8, 9, new TileData(DiggableType.RareGem));
        }

        private void TestSetInvalid()
        {
            diggableData.SetAt(0, 1, new TileData(DiggableType.RareGem));            
        }

        private void TestReduceValid()
        {
            diggableData.ReduceAt(-2, 2, 5);
            diggableData.ReduceAt(-8, 9, 4);
            diggableData.ReduceAt(0, 3, 4);
        }

        private void TestFormat(string testName, System.Action testAction)
        {
            Debug.Log(">--------" + testName + "--------<");
            
            // try
            // {
            //     testAction();
            //     diggableData.Log();
            // }
            // catch (InvalidTileException)
            // {
            //     Debug.LogError("Invalid tile");
            // }
            // finally
            // {
            //     Debug.Log(">---------------------------------------------<");
            // }
        }
    }
}