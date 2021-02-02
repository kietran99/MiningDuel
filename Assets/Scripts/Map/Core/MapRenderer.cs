using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

namespace MD.Map.Core
{
    public class MapRenderer : NetworkBehaviour
    {
        [SerializeField] Tilemap botMap = null;
        [SerializeField] Tilemap topMap = null;
        [SerializeField] Tilemap obstacleMap = null;
        [SerializeField] RuleTile tileNo1 = null;
        [SerializeField] RuleTile tileNo2 = null;
        [SerializeField] RuleTile tileNo3 = null;
        [SerializeField] RuleTile obstacleTile = null;

        public override void OnStartAuthority()
        {
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
            topMap.ClearAllTiles();
            botMap.ClearAllTiles();
            obstacleMap.ClearAllTiles();
            // int width = map.GetLength(0);
            // int height = map.GetLength(1);
            if(map!= null)
            {
                for(int x = 0; x < width; x++)
                {
                    for(int y = 0; y < height; y++)
                    {
                        
                        botMap.SetTile(new Vector3Int(x-width/2, y-height/2, 0), tileNo1);
                        if(map[x*width + y] == 1)
                        {
                            topMap.SetTile(new Vector3Int(x-width/2, y-height/2, 0), tileNo2);
                        }
                        else if(map[x*width + y] == 2)
                        {
                            topMap.SetTile(new Vector3Int(x-width/2, y-height/2, 0), tileNo3);
                        }                  
                        else if(map[x*width + y] == -1)
                        {
                            obstacleMap.SetTile(new Vector3Int(x-width/2, y-height/2, 0), obstacleTile);
                        }                  
                    }
                }
            }


            botMap.CompressBounds();
            Camera.main.GetComponent<CameraController>().SetMapData(botMap);//k chắc
        }
    }
}
