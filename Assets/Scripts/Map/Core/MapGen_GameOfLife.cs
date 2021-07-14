using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;
using UnityEngine.Tilemaps;



namespace MD.Map.Core{
    public class MapGen_GameOfLife : MonoBehaviour, IMapGenerator
    {
        
        // public int GetWidth{get{return width;}}
        public int MapWidth => width; 
        public int MapHeight{get{return height;}}
        public int GetCount{get{return count;}}
        bool useGeneratedMaps = false;
        string mapName = "";
        public bool UseGeneratedMaps => useGeneratedMaps;

        public string mapUsed => mapName ;

        public List<Vector2Int> MovablePostions {get{
            List<Vector2Int> res = new List<Vector2Int>();
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    if(map[x,y] >= 0)
                    {
                        res.Add(new Vector2Int(x,y));
                    }
                }
            }
            return res;
        }}

        public int[] MapData{get;}

        public SpawnPositionsData SpawnPositionsData => throw new NotImplementedException();

        public bool IsObstacle(int x, int y)
        {
            if(map[x,y] == Constants.OBSTACLE)
            {
                return true;
            }
            return false;
        }

        [SerializeField] int width = 0;
        [SerializeField] int height =  0;
        [SerializeField] string seed = "";
        [SerializeField] bool useRandomSeed = false;
        // [SerializeField] int reGenTimes = 0;
        [Range(0,100)] public int randomFillPercent1 = 0;
        [Range(0,100)] public int randomFillPercent2 = 0;
        [Range(1,8)] [SerializeField] int deathLim = 1;
        [Range(1,8)] [SerializeField] int birthLim = 1;
        [SerializeField] Tilemap topMap = null;
        [SerializeField] Tilemap botMap = null;
        [SerializeField] RuleTile tileNo1 = null;
        [SerializeField] RuleTile tileNo2 = null;
        [SerializeField] RuleTile tileNo3 = null;
        int[,] map = null;
        int count = 0;
        int totalFill;
        // int timesRegen=0;
        void Start()
        {
            totalFill = randomFillPercent1 + randomFillPercent2;
            totalFill = (totalFill > 100)? 100: totalFill;
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
            if(Input.GetKeyDown(KeyCode.Space))
            {
                ApplyTiles();
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
            RandomFillMap();
            // timesRegen=0;
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
            // timesRegen+=1;
            // Debug.Log("Times ReGen: " + timesRegen + " State: " + timesRegen%3);
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
                            // if(map[x,y] == 1)
                            //     newMap[x,y] = 2;
                            // else
                            //     newMap[x,y] = 0; 
                            newMap[x,y] = 1;
                        }
                    }
                    if(map[x,y] == 0)
                    {
                        if(aliveNeighbor[0] < birthLim)
                        {
                            // switch(aliveNeighbor[1])
                            // {
                            //     case 1:
                            //         newMap[x,y] = 2;
                            //         break;
                            //     case 2:
                            //         newMap[x,y] = 0;
                            //         break;
                            // }
                            newMap[x,y] = aliveNeighbor[1];
                        }
                        // else
                        // {
                        //     newMap[x,y] = 1;
                        // }
                    }
                    else if( map[x,y] == 1)
                    {
                        newMap[x,y] = 2;
                    }
                    else if(map[x,y] == 2)
                    {
                        newMap[x,y] = 0;
                    }

                    // if(map[x,y] != 0)
                    // {
                    //     if(aliveNeighbor[0] >= deathLim)
                    //     {
                    //         newMap[x,y] = 0;
                    //     }
                    // }
                    // else 
                    // {
                    //     if(aliveNeighbor[0] >= birthLim)
                    //     {
                    //         newMap[x,y] = aliveNeighbor[1];
                    //     }
                    // }
                    // if(newMap[x,y] == 0)
                    // {
                    //     newMap[x,y] = 1;
                    // }
                    // else if(newMap[x,y] == 1)
                    // {
                    //     newMap[x,y] = 2;
                    // }
                    // else
                    // {
                    //     newMap[x,y] =0;
                    // }
                }
            }
            
            return newMap;
        }
        public List<Vector3> SpawnStoragePos()
        {
            return new List<Vector3>();
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
            // if(numOfNo1 == numOfNo2)
            // {
            //     int ran = new Random(seed.GetHashCode()).Next(0,100);
            //     if(ran < 50)
            //     {
            //         neighbor[1] = 1;
            //     }
            //     else
            //     {
            //         neighbor[1] = 2;
            //     }
            // }
            // else if( numOfNo1 > numOfNo2)
            // {
            //     neighbor[1] = 1;
            // }
            // else
            // {
            //     neighbor[1] = 2;
            // }
            neighbor[1] = (numOfNo2 > numOfNo1)? 2:1;
            return neighbor;
        }
        void OnDrawGizmos()
        {
            if(map!= null)
            {
                for(int x = 0; x < width; x++)
                {
                    for(int y = 0; y< height; y++)
                    {
                        switch(map[x,y])
                        {
                            case 0:
                                Gizmos.color = Color.white;
                                break;
                            case 1:
                                Gizmos.color = Color.black;
                                break;
                            case 2:
                                Gizmos.color = Color.blue;
                                break;
                        }
                        Vector3 pos = new Vector3( x+.5f, y +.5f,0);
                        Gizmos.DrawCube(pos,Vector3.one); 
                    }
                }
            }
        }
        void ApplyTiles()
        {
            topMap.ClearAllTiles();
            botMap.ClearAllTiles();
            if(map!= null)
            {
                for(int x = 0; x < width; x++)
                {
                    for(int y = 0; y < height; y++)
                    {
                        
                        botMap.SetTile(new Vector3Int(x , y , 0), tileNo1);
                        if(map[x,y] == 1)
                        {
                            topMap.SetTile(new Vector3Int(x , y , 0), tileNo2);
                        }
                        else if(map[x,y] == 2)
                        {
                            topMap.SetTile(new Vector3Int(x, y, 0), tileNo3);
                        }                  
                    }
                }
            }
        }
    }
}