using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MD.Quirk;

namespace MD.Diggable.Core
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

        [Header("Quirk")]
        [SerializeField]
        private GameObject quirkObtainPrefab = null;

        [SerializeField]
        private GameObject[] spawnableQuirkPrefabs = null;
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

        private SonarTileData[] initSonarTileData;

        public override void OnStartServer()
        {
            ServiceLocator.Register((IDiggableGenerator) this);
            eventBroadcaster = new DiggableEventBroadcaster(this);
            InitSortedNodeBasedSpawnTable();
            ServiceLocator
                .Resolve<MD.Map.Core.IMapGenerator>()
                .Match(

                    errorMessage => Debug.Log(errorMessage.Message), 
                    mapGenerator => 
                    {
                        var tilePositions = mapGenerator.MovablePostions.ToArray();
                        Debug.Log(tilePositions.Length);
                        tileGraph = new TileGraph(tilePositions);
                        diggableData = new DiggableData(MakeEmptyTiles(tilePositions));
                        FillInitSonarTileData(tilePositions);
                        System.Linq.Enumerable.Range(0, startSpawnAmount).ForEach(_ => RandomSpawn());
                        botEventHandler = new BotDiggableEventHandler();
                        StartCoroutine(RandomSpawnOverTime());
                    }
                );
        }
        
        private void FillInitSonarTileData(Vector2Int[] tilePositions)
        {
            initSonarTileData = new SonarTileData[tilePositions.Length];
            for (int i = 0; i < initSonarTileData.Length; i++) 
            {
                initSonarTileData[i] = new SonarTileData(tilePositions[i].x, tilePositions[i].y, DiggableType.EMPTY);
            }
        }
        
        public SonarTileData[] InitSonarTileData => initSonarTileData;

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
            SpawnAt(randEmptyPos, randDiggableType);           
        }

        private void SpawnAt(Vector2Int pos, DiggableType type)
        {
            diggableData
                .GetAccessAt(pos.x, pos.y)
                .Match(
                    invalidTileErr => Debug.Log("Invalid Tile"),
                    access => 
                    {
                        if (type.Equals(DiggableType.QUIRK))
                        {
                            SpawnQuirkObtainAt(pos.x, pos.y, spawnableQuirkPrefabs.Random());
                            return;
                        }

                        tileGraph.OnDiggableSpawn(pos);
                        diggableData.Spawn(access, type);
                        eventBroadcaster.TriggerDiggableSpawnEvent(pos.x, pos.y, type); 
                        (var _, var idx) = initSonarTileData.LookUp(tileData => tileData.x.Equals(pos.x) && tileData.y.Equals(pos.y));   
                        initSonarTileData[idx] = new SonarTileData(initSonarTileData[idx].x, initSonarTileData[idx].y, type);
                    }
                );
        }

        private void SpawnQuirkObtainAt(int x, int y, GameObject quirkPrefab)
        {
            var quirkObtainInstance = 
                Instantiate(quirkObtainPrefab, new Vector3(x + MapConstants.GRID_OFFSET, y + MapConstants.GRID_OFFSET, 0f), Quaternion.identity);
            NetworkServer.Spawn(quirkObtainInstance);
            var quirkInstance = Instantiate(quirkPrefab);
            NetworkServer.Spawn(quirkInstance);
            quirkObtainInstance.GetComponent<QuirkObtain>().RpcInitialize(quirkInstance);
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

        private (Vector2Int, ITileData)[] MakeEmptyTiles(Vector2Int[] positions)
        {
            var tiles = new (Vector2Int, ITileData)[positions.Length];

            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = (positions[i], new TileData(DiggableType.EMPTY));
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

        [ServerCallback]
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                SpawnAt(new Vector2Int(0, 0), DiggableType.RARE_GEM);
                eventBroadcaster.TriggerDiggableSpawnEvent(0, 0, DiggableType.RARE_GEM);
            }

            else if (Input.GetKeyDown(KeyCode.X))
            {               
                // SpawnQuirkObtainAt(5, 5, spawnableQuirkPrefabs.Random());
                // SpawnQuirkObtainAt(7, 7, spawnableQuirkPrefabs.LookUp(prefab => prefab.name.Equals("Mighty Blessing")).item);
                SpawnQuirkObtainAt(7, 7, spawnableQuirkPrefabs.LookUp(prefab => prefab.name.Equals("Drill Machine")).item);
                // SpawnQuirkObtainAt(7, 7, spawnableQuirkPrefabs.LookUp(prefab => prefab.name.Equals("Camo Perse")).item);
            }
        }
    }
}
