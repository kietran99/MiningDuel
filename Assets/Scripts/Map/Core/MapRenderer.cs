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
                    mapGenerator => TargetRender(mapGenerator.MapData, mapGenerator.MapWidth, mapGenerator.MapHeight)
                );
        }
        
        [TargetRpc]
        private void TargetRender(int[] map, int width, int height)
        {
            botMap = grid.transform.GetChild(0).GetComponent<Tilemap>();
            topMap = grid.transform.GetChild(1).GetComponent<Tilemap>();
            obstacleMap = grid.transform.GetChild(2).GetComponent<Tilemap>();
            if(botMap == null) Debug.Log("U Fucked Up");
            Debug.Log("My Client: " + netId);
            topMap.ClearAllTiles();
            Debug.Log("Map: Top cleared");
            botMap.ClearAllTiles();
            Debug.Log("Map: Bottom cleared");
            obstacleMap.ClearAllTiles();
            Debug.Log("Map: Obstacle cleared");

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

            botMap.CompressBounds();
            Camera.main.GetComponent<CameraController>().SetMapData(botMap);
        }
    }
}
