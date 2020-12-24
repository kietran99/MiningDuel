using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MD.Map.Core
{
    public class DiggableGenerator : MonoBehaviour
    {
        #region SERIALIZE FIELDS
        [SerializeField]
        private float generateInterval = 2f;

        [SerializeField]
        private int generateAreaLength = 4;
        #endregion

        #region FIELDS
        private IDiggableData diggableData;
        private TileGraph tileGraph;
        private bool shouldSpawn = false;
        #endregion

        void Start()
        {
            //TestPopulate();
            var tilePositions = GenTestTilePositions();
            tileGraph = new TileGraph(tilePositions);
            diggableData = new DiggableData(MakeEmptyTiles(tilePositions));
            diggableData.Log();
            tileGraph.Log();
            StartCoroutine(RandomSpawn());
        }

        private Vector2Int[] GenTestTilePositions()
        {
            return new Vector2Int[]
            {
                new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-2, 2), 
                new Vector2Int(5, 6), new Vector2Int(8, 9), new Vector2Int(-1, 2)
            };           
        }

        private IEnumerator RandomSpawn()
        {
            shouldSpawn = true;
            var interval = new WaitForSecondsRealtime(generateInterval);

            while (shouldSpawn)
            {
                yield return interval;

                var randEmptyPos = tileGraph.RandomTile();
                var randDiggable = RandomDiggable();
                //Debug.Log("RANDOM RESULT: " + randEmptyPos + " " + randDiggable);
                tileGraph.OnDiggableSpawn(randEmptyPos);
                SpawnAt(randEmptyPos, randDiggable);
                
                diggableData.Log();
                tileGraph.Log();
                //tileGraph.LogExpectedRates();

                if (diggableData.FreeTiles.Count == 0)
                {
                    shouldSpawn = false;
                }
            }           
        }

        private void SpawnAt(Vector2Int pos, DiggableType type)
        {
            diggableData.GetAccessAt(pos.x, pos.y).Match(
                invalidTileErr => Debug.Log("Invalid Tile"),
                access => diggableData.Spawn(access, type)
            );
        }

        private (Vector2Int, ITileData)[] MakeEmptyTiles(Vector2Int[] positions)
        {
            var tiles = new (Vector2Int, ITileData)[positions.Length];

            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = (positions[i], new TileData(DiggableType.Empty));
            }

            return tiles;
        }

        public void Populate(Vector2Int[] tilePositions)
        {
            diggableData.Populate(MakeRandomTiles(tilePositions));
        }      

        private (Vector2Int, ITileData)[] MakeRandomTiles(Vector2Int[] positions)
        {
            var tiles = new (Vector2Int, ITileData)[positions.Length];

            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = (positions[i], RandomTileData());
            }

            return tiles;
        }

        private TileData RandomTileData()
        {           
            return new TileData(RandomDiggable());
        }   

        private DiggableType RandomDiggable()
        {
            var diggableTypes = Enum.GetValues(typeof(DiggableType));
            List<DiggableType> typeList = new List<DiggableType>();
            foreach (var i in diggableTypes)
            {
                if (((DiggableType) i).Equals(DiggableType.Empty)) continue;
                typeList.Add((DiggableType) i);
            }
            //return (DiggableType) diggableTypes.GetValue(UnityEngine.Random.Range(0, diggableTypes.Length));
            return typeList[UnityEngine.Random.Range(0, typeList.Count)];
        }    
    }
}
