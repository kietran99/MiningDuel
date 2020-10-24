using UnityEngine;

namespace MD.Diggable.Core
{
    public class MapTester : MonoBehaviour
    {
        void Start()
        {
            IMap map = new Map(
                new Tile[4]
                {
                    new Tile(1, 2, new TileData(DiggableType.CommonGem)),
                    new Tile(-1, 3, new TileData(DiggableType.UncommonGem)),
                    new Tile(2, -4, new TileData(DiggableType.RareGem)),
                    new Tile(-4, -5, new TileData(DiggableType.CommonGem))
                }
            );

            map.TrySet(-4, -5, new TileData(DiggableType.Empty));
            map.Log();

            if (map.TryGet(2, -4, out TileData data))
            {
                Debug.Log(data);
            }
        }
    }
}