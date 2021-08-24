using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

namespace MD.Map.Core
{
    public class MapRenderer : NetworkBehaviour
    {
        Tilemap botMap = null;
        Tilemap topMap = null;
        Tilemap obstacleMap = null;
        [SerializeField] Grid grid = null;
        [SerializeField] RuleTile tileNo1 = null;
        [SerializeField] RuleTile tileNo2 = null;
        [SerializeField] RuleTile tileNo3 = null;
        [SerializeField] RuleTile obstacleTile = null;
        public override void OnStartAuthority()
        {
            // Instantiate(grid);
            grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
            CmdRequestRender();
        }

        [Command]
        private void CmdRequestRender()
        {
            ServiceLocator
                .Resolve<IMapGenerator>()
                .Match(
                    errMessage => Debug.Log(errMessage.Message), 
                    mapGenerator => TargetRender(mapGenerator.MapData,mapGenerator.ObstacleData, mapGenerator.MapWidth, mapGenerator.MapHeight, mapGenerator.UseGeneratedMaps, mapGenerator.mapUsed)
                );
        }
        
        [TargetRpc]
        private void TargetRender(int[] map,int[] obstacleData, int width, int height, bool useGeneratedMaps, string mapName)
        {
            if(useGeneratedMaps)
            {
                Destroy(grid);
                string mapPath = "GeneratedMaps/"+mapName;
                // Debug.Log("Map Name: "+mapPath);
                grid = (Instantiate(Resources.Load(mapPath,typeof(GameObject))) as GameObject).GetComponent<Grid>();
                botMap = grid.transform.GetChild(0).GetComponent<Tilemap>();
                botMap.CompressBounds();
                Camera.main.GetComponent<CameraController>().SetMapData(botMap);
                return;
            }
            botMap = grid.transform.GetChild(0).GetComponent<Tilemap>();
            topMap = grid.transform.GetChild(1).GetComponent<Tilemap>();
            obstacleMap = grid.transform.GetChild(2).GetComponent<Tilemap>();
            topMap.ClearAllTiles();
            botMap.ClearAllTiles();
            obstacleMap.ClearAllTiles();

            if(map!= null)
            {
                for(int x = 0; x < width; x++)
                {
                    for(int y = 0; y < height; y++)
                    {
                        
                        botMap.SetTile(new Vector3Int(x, y, 0), tileNo1);
                        if(map[x*width + y] == 1)
                        {
                            topMap.SetTile(new Vector3Int(x, y, 0), tileNo2);
                        }
                        else if(map[x*width + y] == 2)
                        {
                            topMap.SetTile(new Vector3Int(x, y, 0), tileNo3);
                        }                  
                        else if(map[x*width + y] == -1)
                        {
                            obstacleMap.SetTile(new Vector3Int(x, y, 0), obstacleTile);
                        }                  
                    }
                }
            }
            if(obstacleData != null)
            {
                for(int x = 0;x < width ; x++)
                {
                    for(int y = 0;y < height; y++)
                    {
                        if( x > width /2 - 5 && x < width /2 + 5 && y > height /2 - 5 && y < height /2 + 5)
                        {
                            continue;
                        }
                        if(obstacleData[x*width + y] == -1)
                        {
                            obstacleMap.SetTile(new Vector3Int(x, y, 0), obstacleTile);
                        }  
                    }
                }
            }


            // SetUpWalls around map
            for(int x = -1 ; x <= width; x++)
            {
                obstacleMap.SetTile(new Vector3Int(x,-1,0),obstacleTile);
                obstacleMap.SetTile(new Vector3Int(x,height,0),obstacleTile);
            }
            for(int y = 0 ; y < height; y++)
            {
                obstacleMap.SetTile(new Vector3Int(-1,y,0),obstacleTile);
                obstacleMap.SetTile(new Vector3Int(width,y,0),obstacleTile);
            }
            
            botMap.CompressBounds();
            Camera.main.GetComponent<CameraController>().SetMapData(botMap);
        }
    }
}
