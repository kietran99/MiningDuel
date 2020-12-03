using System;
using UnityEngine;

namespace MD.Map.Core
{
    [RequireComponent(typeof(DiggableData))]
    public class DiggableGenerator : MonoBehaviour
    {
        private IDiggableData diggableData;

        void Start()
        {
            diggableData = GetComponent<IDiggableData>();
        }

        public void Populate(Vector2Int[] tilePositions)
        {
            diggableData.Populate(MakeRandTiles(tilePositions));
        }

        private (Vector2Int, ITileData)[] MakeRandTiles(Vector2Int[] positions)
        {
            var tiles = new (Vector2Int, ITileData)[positions.Length];

            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = (positions[i], GenRandomTileData());
            }

            return tiles;
        }

        private TileData GenRandomTileData()
        {
            var diggableTypes = Enum.GetValues(typeof(DiggableType));
            var randType = (DiggableType) UnityEngine.Random.Range(0, diggableTypes.Length);
            
            return new TileData(randType);
        }
    }
}
