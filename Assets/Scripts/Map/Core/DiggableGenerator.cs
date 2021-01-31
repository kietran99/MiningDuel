using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MD.Map.Core
{
    [RequireComponent(typeof(ProjectileSpawner))]
    public class DiggableGenerator : NetworkBehaviour, IDiggableGenerator
    {
        [Serializable]
        public struct SpawnRate
        {
            public DiggableType type;
            public int weight;
        }

        #region SERIALIZE FIELDS
        [SerializeField]
        private SpawnRate[] spawnTable = null;

        [Header("Spawn Stats")]
        [SerializeField]
        private int startSpawnAmount = 10;

        [SerializeField]
        private float generateInterval = 2f;
        #endregion

        #region FIELDS
        private IDiggableData diggableData;
        private TileGraph tileGraph;
        private DiggableEventBroadcaster eventBroadcaster;
        private BotDiggableEventHandler botEventHandler;
        private List<WeightedNode<DiggableType>> nodeBasedSpawnTable;
        #endregion

        #region EVENTS
        public Action<Mirror.NetworkConnection, Diggable.Gem.DigProgressData> DigProgressEvent { get; set; }
        public Action<Mirror.NetworkConnection, Diggable.Gem.GemObtainData> GemObtainEvent { get; set; }
        public Action<Mirror.NetworkConnection, Diggable.Projectile.ProjectileObtainData> ProjectileObtainEvent { get; set; }
        public Action<Diggable.DiggableRemoveData> DiggableDestroyEvent { get; set; }
        public Action<Diggable.DiggableSpawnData> DiggableSpawnEvent { get; set; }
        #endregion

        public override void OnStartServer()
        {
            ServiceLocator.Register((IDiggableGenerator) this);
            eventBroadcaster = new DiggableEventBroadcaster(this);
            botEventHandler = new BotDiggableEventHandler();
            InitSortedNodeBasedSpawnTable();
            var tilePositions = GenerateDefaultMap();
            tileGraph = new TileGraph(tilePositions);
            diggableData = new DiggableData(MakeEmptyTiles(tilePositions));
            System.Linq.Enumerable.Range(0, startSpawnAmount).ForEach(_ => RandomSpawn());
            // diggableData.Log();
            // tileGraph.Log();
            StartCoroutine(RandomSpawnOverTime());
        }

        private void InitSortedNodeBasedSpawnTable()
        {
            nodeBasedSpawnTable = new List<WeightedNode<DiggableType>>();
            var duplicateCheckSet = new HashSet<DiggableType>();            

            spawnTable
                .ForEach(spawnRate => 
                    {
                        if (duplicateCheckSet.Contains(spawnRate.type))
                        {
                            return;
                        }

                        nodeBasedSpawnTable.Add(new WeightedNode<DiggableType>(spawnRate.type, spawnRate.weight));
                        duplicateCheckSet.Add(spawnRate.type);
                    }
                );

            nodeBasedSpawnTable.SortH2LByWeight();
            nodeBasedSpawnTable.LogExpectedRates();
        }

        private Vector2Int[] GenerateDefaultMap()
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

        private IEnumerator RandomSpawnOverTime()
        {
            bool shouldSpawn = true;
            var interval = new WaitForSecondsRealtime(generateInterval);

            while (shouldSpawn)
            {
                yield return interval;

                RandomSpawn();               
                shouldSpawn = diggableData.FreeTiles.Count != 0;               
            }           
        }

        private void RandomSpawn()
        {
            var randEmptyPos = tileGraph.RandomTile();
            var randDiggableType = nodeBasedSpawnTable.RandomSortedList();
            // Debug.Log("Spawn: " + randDiggableType + " at " + randEmptyPos);
            tileGraph.OnDiggableSpawn(randEmptyPos);
            SpawnAt(randEmptyPos, randDiggableType);
            eventBroadcaster.TriggerDiggableSpawnEvent(randEmptyPos.x, randEmptyPos.y, randDiggableType);        
            //diggableData.Log();
            //tileGraph.Log();
            //tileGraph.LogExpectedRates();
        }

        public void Populate(Vector2Int[] tilePositions)
        {
            diggableData = new DiggableData(MakeEmptyTiles(tilePositions));
        }

        public void DigAt(Mirror.NetworkIdentity digger, int x, int y, int power)
        {
            diggableData
                .GetAccessAt(x, y)
                .Match(
                    invalidTileErr => Debug.LogError(invalidTileErr.Message),
                    access => 
                    {
                        diggableData
                            .Reduce(access, power)
                            .Match(                           
                                invalidAccessErr => Debug.LogError(invalidAccessErr.Message),
                                reducedData => 
                                {                                    
                                    eventBroadcaster.TriggerDiggableDugEvent(digger, reducedData);
                                    
                                    if (reducedData.isEmpty) 
                                    {
                                        eventBroadcaster.TriggerDiggableDestroyEvent(x, y);                                    
                                    }
                                }                                    
                            );
                    }
                );
        }

        public void BotDigAt(MD.AI.PlayerBot bot, int x, int y, int power)
        {
            diggableData
                .GetAccessAt(x, y)
                .Match(
                    invalidTileErr => Debug.LogError(invalidTileErr.Message),
                    access => 
                    {
                        diggableData
                            .Reduce(access, power)
                            .Match(                           
                                invalidAccessErr => Debug.LogError(invalidAccessErr.Message),
                                reducedData => 
                                {                                                                       
                                    if (!reducedData.isEmpty) return;
                                    
                                    botEventHandler.HandleDiggableDugEvent(bot, reducedData);
                                    eventBroadcaster.TriggerDiggableDestroyEvent(x, y);                                                                        
                                }                                    
                            );
                    }
                );
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
    
        public DiggableType[] GetDiggableArea(Vector2Int[] positions)
        {
            var diggableArea = new DiggableType[positions.Length];

            for (int i = 0, size = diggableArea.Length; i < size; i++)
            {
                diggableData
                    .GetDataAt(positions[i].x, positions[i].y)
                    .Match(
                        err => Debug.LogWarning(err.Message),
                        tileData => diggableArea[i] = tileData.Type
                    );
            }

            return diggableArea;
        }
    
        public Functional.Type.Either<InvalidTileError, bool> IsProjectileAt(int x, int y)
        {
            return diggableData.GetDataAt(x, y).Map(tileData => tileData.Type.IsProjectile());
        }

        public Functional.Type.Either<InvalidTileError, bool> IsGemAt(int x, int y)
        {
            return diggableData.GetDataAt(x, y).Map(tileData => tileData.Type.IsGem());
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                SpawnAt(new Vector2Int(0, 0), DiggableType.RareGem);
                eventBroadcaster.TriggerDiggableSpawnEvent(0, 0, DiggableType.RareGem);
            }

            else if (Input.GetKeyDown(KeyCode.X))
            {
                
            }
        }
    }
}
