﻿using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using Random = System.Random;

namespace MD.Map.Core
{
    public class SpawnPositionsData
    {
        private int idx;
        private Vector2[] spawnPositions;

        public SpawnPositionsData(Vector2[] spawnPositions)
        {
            this.idx = -1;
            this.spawnPositions = spawnPositions;
        }

        public Vector2[] SpawnPositions => spawnPositions;

        public Vector2 NextSpawnPoint
        {
            get
            {
                idx++;        
                return spawnPositions[idx];            
            }
        }
    }
    public struct ChunkObstacle
    {
        private int[,] obstaPositions;
        private int size;
        public ChunkObstacle(int[,] pos)
        {
            obstaPositions = pos;
            size = obstaPositions.GetLength(0);
        }
        public bool Available(int rootX, int rootY, int[,] map)
        {
            if (map == null || rootX < 0 || rootY < 0) return false;
            // int size = obstaPositions.GetLength(0);
            for(int i = 0; i < size; i++)
            {
                int xPos = rootX + obstaPositions[i,0];
                int yPos = rootY + obstaPositions[i,1];
                if(xPos >= map.GetLength(0) || yPos >= map.GetLength(1)) return false;
                // if ( xPos == 0 || xPos == map.GetLength(0) - 1 || yPos == 0 || yPos == map.GetLength(1) - 1) return false;
                // if(InCornerArea(xPos,yPos,map)) return false;
                if(map[xPos,yPos] == -1)
                    return false;
            }
            return true;
        }
        private bool InCornerArea(int x, int y, int[,] map)
        {

            return false;
        }
        public int[,] Positions => obstaPositions;
        public int Size => size;
    }

    public class MapGenerator : NetworkBehaviour, IMapGenerator
    { 
        [SerializeField] 
        bool useGeneratedMaps = false;

        [SerializeField] 
        string[] allMapsName = null;

        [Range(0f, 100f)]
        [SerializeField]
        private float obstacleFillRate = 30f;

        [SerializeField] 
        int noObstacleAreaRadius = 4;

        [SerializeField]
        private Vector2 _cornerSpawnOffset = Vector2.zero;

        string mapName = "";

        public int MapWidth => width;
        public int MapHeight => height;
        public bool UseGeneratedMaps => useGeneratedMaps;
        public int[] ObstacleData 
        {
            get
            {
                int[] simpleData = new int[width * height];
                for(int x = 0; x < width; x++)
                {
                    for(int y = 0; y < height; y++)
                    {
                        simpleData[x*width + y] = obstacleData[x,y];
                    }
                }

                return simpleData;
            }
        }

        public string mapUsed => mapName;
        
        public SpawnPositionsData SpawnPositionsData 
        {
            get
            {
                var spawnPosList = new Vector2[]
                {
                    new Vector2(_cornerSpawnOffset.x, _cornerSpawnOffset.y),
                    new Vector2(width - _cornerSpawnOffset.x , height - _cornerSpawnOffset.y),
                    new Vector2(width - _cornerSpawnOffset.x , _cornerSpawnOffset.y),
                    new Vector2(_cornerSpawnOffset.x, height - _cornerSpawnOffset.y)
                };

                return new SpawnPositionsData(spawnPosList);
            }
        }

        [SerializeField] int width = 0;
        [SerializeField] int height =  0;
        [SerializeField] string seed = "";
        [SerializeField] bool useRandomSeed = true;
        // [SerializeField] int reGenTimes = 0;
        [Range(0,100)] public int randomFillPercent1 = 0;
        [Range(0,100)] public int randomFillPercent2 = 0;
        [Range(1,8)] [SerializeField] int deathLim = 1;
        [Range(1,8)] [SerializeField] int birthLim = 1;
        // [Range(1,5)] [SerializeField] int emptyRadius = 3;

        int[,] map = null; 
        int totalFill;
        int[,] obstacleData = null;

        // ChunkObstacle chunksT = new ChunkObstacle(new int[,]{{0,0},{1,0},{2,0},{1,1}});
        ChunkObstacle[] chunks = new ChunkObstacle[] {  new ChunkObstacle(new int[,] {{0,0},{1,0},{2,0},{1,1}}),
                                                        new ChunkObstacle(new int[,] {{0,0},{1,0},{2,0},{2,1}}),
                                                        new ChunkObstacle(new int[,] {{0,0},{1,0},{2,0},{0,1}}),
                                                        // new ChunkObstacle(new int[,] {{0,0},{2,0},{1,1},{0,2},{2,2}}),
                                                        new ChunkObstacle(new int[,] {{1,0},{0,1},{1,1},{2,1},{1,2}})};

        List<Vector3> avail_storage_pos = null;
        private Vector2Int[] _storagePosList;

        public override void OnStartServer()
        {
            ServiceLocator.Register((IMapGenerator)this);  
            if (useGeneratedMaps && allMapsName.Length > 0)
            {
                // Destroy(GameObject.FindGameObjectWithTag("Grid"));
                int rd = UnityEngine.Random.Range(0, allMapsName.Length);
                MapData mapData = SaveMapData.LoadMap(allMapsName[rd]);
                mapName = allMapsName[rd];
                width = mapData.width;
                height = mapData.height;
                map = new int[width,height];
                obstacleData = new int[width,height];
                for(int x = 0; x < width; x++)
                {
                    for(int y =0; y < height; y++)
                    {
                        map[x,y] = mapData.GetElement(x,y);
                        obstacleData[x,y] = map[x,y]; 
                    }
                }
                // obstacleData = map.Clone();
                avail_storage_pos = SpawnStoragePos();  
                return;
            }
            totalFill = randomFillPercent1 + randomFillPercent2;
            totalFill = (totalFill > 100)? 100: totalFill;
            GenerateMap();
            if (useRandomSeed)
            {
                seed = Time.time.ToString();
            }
            Random pseudoRandom = new Random(seed.GetHashCode());
            int random = pseudoRandom.Next(1,10);
            for(int i = 0; i < random*3+1; i++)
            {
                map = Smoothening();
            }

            AddObstacle();     
            avail_storage_pos = SpawnStoragePos();   
        }





        public int[] MapData 
        {
            get
            {
                int[] simpleData = new int[width * height];
                for(int x = 0; x < width; x++)
                {
                    for(int y = 0; y < height; y++)
                    {
                        simpleData[x*width + y] = map[x,y];
                    }
                }

                return simpleData;
            }
        }


        public List<Vector3> SpawnStoragePos()
        {
            List<Vector3> lst = new List<Vector3>();
            for(int x = width/2 - noObstacleAreaRadius + 2; x <= width/2 + noObstacleAreaRadius - 2; x++)
            {
                for(int y = height/2 - noObstacleAreaRadius + 2; y <= height/2 + noObstacleAreaRadius - 2; y++)
                {
                    if(!(IsObstacle(x,y)))
                    {
                        lst.Add(new Vector3(x,y,0));
                    }
                }
            }

            // _storagePosList = ;
            return lst;
        }
        public Vector3 SpawnStorage()
        {
            int rnd = UnityEngine.Random.Range(0,avail_storage_pos.Count);
            Vector3 res = avail_storage_pos[rnd];
            obstacleData[((int)res.x),((int)res.y)] = -1;
            avail_storage_pos.RemoveAt(rnd);
            return res;

        }

        public void UpdateObsatcleData(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
            {
                return;
            }
            obstacleData[x,y] = -1;
        }

        public bool IsObstacle(int x, int y)
        {
            if(x < 0 || x>= MapWidth || y < 0 || y >= MapHeight)
            {
                // Debug.LogError("Negative index on Obstacle check! x= " + x + " y= " + y);
                return true;
            }
            // if(map[x,y] == Constants.BLOCK)

            // if (_storagePosList != null)
            // {
            //     foreach (var storagePos in _storagePosList)
            //     {
            //         if (storagePos.x == x && storagePos.y == y)
            //         {
            //             return true;
            //         }
            //     }
            // }

            if(map == null) 
            {
                return true;
            }

            if(obstacleData[x,y] < 0)
            {
                return true;
            }

            return false;
        }

        public List<Vector2Int> MovablePostions 
        {
            get
            {
                List<Vector2Int> res = new List<Vector2Int>();
                for(int x = 0; x < width; x++)
                {
                    for(int y = 0; y < height; y++)
                    {
                        // if(map[x,y] >= 0)
                        if(!IsObstacle(x,y))
                        {
                            if (!InCornerArea(x,y))
                            {
                                res.Add(new Vector2Int(x,y));
                            }
                        }
                    }
                }

                return res;
            }
        }

        #if UNITY_EDITOR
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.M))
            {
                map = Smoothening();
            }
            if(Input.GetKeyDown(KeyCode.N))
            {
                GenerateMap();
                if(useRandomSeed)
                {
                    seed = Time.time.ToString();
                }
                Random pseudoRandom = new Random(seed.GetHashCode());
                int random = pseudoRandom.Next(1,10);
                for(int i = 0; i < random*3+1; i++)
                {
                    map = Smoothening();
                }
            }

            
            Camera.main.transform.Translate(new Vector3(0,0,Input.GetAxis("Mouse ScrollWheel")*10));
            if(Input.GetKey(KeyCode.LeftArrow))
            {
                Camera.main.transform.Translate(new Vector3(-10*Time.deltaTime,0,0));
            }
            if(Input.GetKey(KeyCode.RightArrow))
            {
                Camera.main.transform.Translate(new Vector3(10*Time.deltaTime,0,0));
            }
            if(Input.GetKey(KeyCode.UpArrow))
            {
                Camera.main.transform.Translate(new Vector3(0,10*Time.deltaTime,0));
            }
            if(Input.GetKey(KeyCode.DownArrow))
            {
                Camera.main.transform.Translate(new Vector3(0,-10*Time.deltaTime,0));
            }
        }

        #endif
        

        void AddObstacle()
        {
            Random random = new Random();
            var fillMult = 10f / GetAvgObsChunkSize(chunks);

            for(int x = 0; x < width; x++)
                for(int y = 0; y < height; y++)
                {
                    if ((x <= width / 2 + noObstacleAreaRadius && x >= width / 2 - noObstacleAreaRadius)&&(y <= height / 2 + noObstacleAreaRadius && y >= height / 2 - noObstacleAreaRadius))
                    {
                        continue;
                    }
                    int chance = random.Next(1,1000);
                    if (chance <= obstacleFillRate * fillMult)
                    {
                        // map[x,y] = -1;
                        AddChunkObstacle(x,y);
                    }
                }
        }

        private float GetAvgObsChunkSize(ChunkObstacle[] chunks)
        {
            var (cnt, sum) = (0, 0);

            foreach (var chunk in chunks)
            {
                sum += chunk.Size;
                cnt++;
            }

            return ((float) sum) / ((float)cnt); 
        }

        void AddChunkObstacle(int x, int y)
        {
            if(InCornerArea(x,y))
            {
                return;
            }
            ChunkObstacle theChosenOne = chunks[UnityEngine.Random.Range(0,chunks.Length)];
            if(theChosenOne.Available(x,y,obstacleData))
            {
                int size = theChosenOne.Size;
                for(int i = 0; i < size; i++)
                {
                    obstacleData[x + theChosenOne.Positions[i,0], y + theChosenOne.Positions[i,1]] = -1;
                }
            }
        }

        void GenerateMap()
        {
            if(map == null)
            {
                map = new int[width,height];
            }
            else
            {
                totalFill = randomFillPercent1 + randomFillPercent2;
                totalFill = (totalFill > 100)? 100: totalFill;
                Array.Clear(map,0,map.Length);
            }
            obstacleData = new int[width,height];
            RandomFillMap();
        }
        void RandomFillMap()
        {
            if(useRandomSeed)
            {
                seed = Time.time.ToString();
            }
            Random pseudoRandom = new Random(seed.GetHashCode());
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    int res = pseudoRandom.Next(0,100);
                    if(res <  randomFillPercent1)
                    {
                        map[x,y] = 1;
                    }
                    else if (res < totalFill)
                    {
                        map[x,y] = 2;
                    }
                    else
                    {
                        map[x,y] = 0;
                    }
                }
            }
        }
        int[,] Smoothening()
        {
            int[,] newMap = map.Clone() as int[,];
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    int[] aliveNeighbor = GetNeighBor(x,y); 
                    if(map[x,y] != 0)
                    {
                        if(aliveNeighbor[0] < deathLim)
                        {
                            if(aliveNeighbor[1] == 1)
                            {
                                newMap[x,y] = 2;
                            }
                            else
                            {
                                newMap[x,y] = 0;
                            }
                        }
                        else
                        {
                            newMap[x,y] = 1;
                        }
                    }
                    if(map[x,y] == 0)
                    {
                        if(aliveNeighbor[0] < birthLim)
                        {
                            newMap[x,y] = aliveNeighbor[1];
                        }
                    }
                    else if( map[x,y] == 1)
                    {
                        newMap[x,y] = 2;
                    }
                    else if(map[x,y] == 2)
                    {
                        newMap[x,y] = 0;
                    }
                }
            }
            
            return newMap;
        }
        int[] GetNeighBor(int posX, int posY)
        {
            int[] neighbor = new int[2];
            int count = 0;
            // int indicator = 0;
            int numOfNo1 = 0;
            int numOfNo2 = 0;
            for(int neighborX = posX - 1; neighborX < posX + 2; neighborX++)
            {
                for(int neighborY = posY - 1; neighborY < posY + 2; neighborY++)
                {
                    if(neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY <height)
                    {
                        if(neighborX != posX || neighborY != posY)
                        {
                            int pos = map[neighborX,neighborY];
                            if(pos != 0)
                            {
                                count++;
                                if(pos == 1)
                                {
                                    numOfNo1++;
                                }
                                else
                                {
                                    numOfNo2++;
                                }
                            }
                        }
                    }
                    else
                    {
                        count++;
                    }
                }
            }
            neighbor[0] = count;
            neighbor[1] = (numOfNo2 > numOfNo1)? 2:1;
            return neighbor;
        }

        bool InCornerArea(int x, int y)
        {
            if(((x>=0 && x <= _cornerSpawnOffset.x)||(x>= width - _cornerSpawnOffset.x - 3 && x < width))&&((y>=0 && y <= _cornerSpawnOffset.y)||(y>= height - _cornerSpawnOffset.y - 3 && y < height)))
            {
                return true;
            }
            return false;
        }

        #region Delete these when release
        // void OnDrawGizmos()
        // {
        //     if(map!= null)
        //     {
        //         for(int x = 0; x < width; x++)
        //         {
        //             for(int y = 0; y< height; y++)
        //             {
        //                 switch(map[x,y])
        //                 {
        //                     case 0:
        //                         Gizmos.color = Color.white;
        //                         break;
        //                     case 1:
        //                         Gizmos.color = Color.black;
        //                         break;
        //                     case 2:
        //                         Gizmos.color = Color.blue;
        //                         break;
        //                 }
        //                 Vector3 pos = new Vector3( x+.5f, y +.5f,0);
        //                 Gizmos.DrawCube(pos,Vector3.one); 
        //             }
        //         }
        //     }
        // }
        // void ApplyTiles()
        // {
        //     topMap.ClearAllTiles();
        //     botMap.ClearAllTiles();
        //     if(map!= null)
        //     {
        //         for(int x = 0; x < width; x++)
        //         {
        //             for(int y = 0; y < height; y++)
        //             {
                        
        //                 botMap.SetTile(new Vector3Int(x , y , 0), tileNo1);
        //                 if(map[x,y] == 1)
        //                 {
        //                     topMap.SetTile(new Vector3Int(x , y , 0), tileNo2);
        //                 }
        //                 else if(map[x,y] == 2)
        //                 {
        //                     topMap.SetTile(new Vector3Int(x, y, 0), tileNo3);
        //                 }                  
        //             }
        //         }
        //     }
        // }

        #endregion
    }
}

