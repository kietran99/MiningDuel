using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventSystems;
using Mirror;
using MD.Diggable.Gem;

namespace MD.Map.Core
{
    //[RequireComponent(typeof(EventConsumer))]
    public class DiggableGenerator : NetworkBehaviour, IDiggableGenerator
    {
        #region SERIALIZE FIELDS
        [SerializeField]
        private int startSpawnAmount = 10;

        [SerializeField]
        private float generateInterval = 2f;
        #endregion

        #region FIELDS
        private EventConsumer eventConsumer;
        private IDiggableData diggableData;
        private TileGraph tileGraph;
        #endregion

        public override void OnStartServer()
        {
            ServiceLocator.Register((IDiggableGenerator) this);
            var tilePositions = GenTestMap();
            tileGraph = new TileGraph(tilePositions);
            diggableData = new DiggableData(MakeEmptyTiles(tilePositions));
            System.Linq.Enumerable.Range(0, startSpawnAmount).ForEach(_ => SetupRandomDiggable());
            // diggableData.Log();
            // tileGraph.Log();
            // ListenToEvents();
            // StartCoroutine(RandomSpawn());
        }

        // private void ListenToEvents()
        // {
        //     eventConsumer = GetComponent<EventConsumer>();
        //     eventConsumer.StartListening<DigRequestData>(HandleDigRequest);
        // }

        public void SetTile(Vector2Int pos, DiggableType type)
        {      
            if (type.Equals(DiggableType.Empty)) return;

            diggableData
                .GetAccessAt(pos.x, pos.y)
                .Match(
                    invalidTileError => Debug.LogError(InvalidTileError.MESSAGE),
                    access => diggableData.Spawn(access, type));
        }

        public void Populate(Vector2Int[] tilePositions)
        {
            diggableData = new DiggableData(MakeEmptyTiles(tilePositions));
        }

        // private void HandleDigRequest(DigRequestData reqData) => DigAt(reqData.x, reqData.y, reqData.power);

        public void DigAt(int x, int y, int power, uint diggerID)
        {
            //Debug.Log("Try digging at: " + x + " : " + y);

            diggableData
                .GetAccessAt(x, y)
                .Match(
                    err => Debug.LogError(InvalidTileError.MESSAGE),
                    access => 
                    {
                        var reducedData = diggableData.Reduce(access, power);
                        Debug.Log("Current: " + reducedData.current + " Max: " + reducedData.max);
                        DiggableEventTrigger.TriggerEvent(reducedData.type, diggerID, reducedData.current, reducedData.max);
                    }
                );
        }

        private Vector2Int[] GenTestMap()
        {
            var (longEdgeLen, shortEdgeLen) = (24, 20);
            var map = new Vector2Int[longEdgeLen * shortEdgeLen];
            int cnt = 0;

            for (int i = -longEdgeLen / 2; i < longEdgeLen / 2; i++)
            {
                for (int j = -shortEdgeLen / 2; j < shortEdgeLen / 2; j++)
                {
                    map[cnt] = new Vector2Int(i, j);
                    cnt++;
                }
            }

            return map;
        }

        private void SetupRandomDiggable()
        {
            var randEmptyPos = tileGraph.RandomTile();
            var randDiggable = RandomDiggable();
            // Debug.Log("Random Result: " + randEmptyPos + " " + randDiggable);
            tileGraph.OnDiggableSpawn(randEmptyPos);
            SpawnAt(randEmptyPos, randDiggable);
            // Instantiate(gemPrefab, new Vector3(randEmptyPos.x, randEmptyPos.y, 0f), Quaternion.identity);
            
            //diggableData.Log();
            //tileGraph.Log();
            //tileGraph.LogExpectedRates();
        }

        private IEnumerator RandomSpawn()
        {
            bool shouldSpawn = true;
            var interval = new WaitForSecondsRealtime(generateInterval);

            while (shouldSpawn)
            {
                yield return interval;

                SetupRandomDiggable();               
                shouldSpawn = diggableData.FreeTiles.Count == 0;               
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

        private (Vector2Int, ITileData)[] RandomTiles(Vector2Int[] positions)
        {
            var tiles = new (Vector2Int, ITileData)[positions.Length];

            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = (positions[i], RandomTileData());
            }

            return tiles;
        }

        private TileData RandomTileData() => new TileData(RandomDiggable()); 

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
    
        public DiggableType[] GetDiggableArea(Vector2Int[] positions)
        {
            var diggableArea = new DiggableType[positions.Length];

            for (int i = 0, size = diggableArea.Length; i < size; i++)
            {
                diggableData
                    .GetDataAt(positions[i].x, positions[i].y)
                    .Match(
                        err => Debug.LogError(InvalidTileError.MESSAGE),
                        tileData => diggableArea[i] = tileData.Type
                    );
            }

            return diggableArea;
        }
    }

    public static class DiggableEventTrigger
    {
        private static Dictionary<DiggableType, System.Action<uint, int, int>> eventTriggerDict = 
            new Dictionary<DiggableType, System.Action<uint, int, int>>()
            {
                { DiggableType.CommonGem, TriggerGemDugEvent },
                { DiggableType.UncommonGem, TriggerGemDugEvent },
                { DiggableType.RareGem, TriggerGemDugEvent },
                { DiggableType.NormalBomb, TriggerProjectileDugEvent },
                { DiggableType.Empty, (diggerID, cur, max) => { UnityEngine.Debug.Log("Dug Empty Tile"); } }
            };

        private static void TriggerGemDugEvent(uint diggerID, int cur, int max)
        {
            Debug.Log("Trigger Gem Dug Event");
            EventManager.Instance.TriggerEvent(new DigProgressData(cur, max));
            if (cur <= 0) EventManager.Instance.TriggerEvent(new MD.Diggable.Gem.GemDigData(diggerID, max));
        }

        private static void TriggerProjectileDugEvent(uint diggerID, int cur, int max)
        {
            UnityEngine.Debug.Log("Trigger Projectile Dug Event");
        } 

        public static void TriggerEvent(DiggableType diggableType, uint diggerID, int cur, int max)
        {
            eventTriggerDict[diggableType](diggerID, cur, max);
        }   
    }
}
