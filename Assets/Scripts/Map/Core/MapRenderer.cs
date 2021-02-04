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
                    mapGenerator => RpcRender(mapGenerator.MapData, mapGenerator.MapWidth, mapGenerator.MapHeight)
                );
        }
// mà nó bậy ntn m? đù t thấy message của m bên chat nhưng t đ vào chat dc
// cài extension r, mà có khác mịa gì đâu  :V
// mà cái map bậy ntn m?
// thế này nhé, nó có 1 map đc generate ra rồi, xong cái có cái map cũ đè lên, sonar chạy trên đấy ok, 
//nhưng ra map chính k đc, then, t thoát thg guest ra, thì bên host nó mất cái map cũ, xong sonar hỏng luôn
// ra map chính k dc là s m? map cũ đè lên là cái map t vẽ sẵn à?
// cái map vẽ sẵn nó đè lên map đc generate
// hmmmm lạ ta, ok chỉ có 1 cách để biết
// debug nó ra, m xem nó debug ra cái g r báo t
// bên host 1 line : 11
// bên guest 1 line : 12
//hmmmmm v thì đâu có g bất thường ta
// để t thử cách của t :V
// ok go go go
// wei thử pha đập đá này xem
// t phát hiện 1 cái khá vui tính
// t để bộ grid trong maprenderer, nhưng chỉ có cái id của nó mới render, còn cái của thg khác nó ở đó để đè lên cái map t sinh
// thay cái TargetRpc thành ClientRpc xong đổi tên TargetRender thành RpcRender
// à ok t biết tại s r
// surprise moda fackas =))
// Do targetRPC nên trên 1 client, chỉ có đúng cái mapRenderer có authority là render, còn cái mapRenderer k có authority nó giữ nguyên data
// Do đó có 2 gridmap trên scene, cái có authority đặt ở dưới, cái k có authority đè lên trên
// simple as dat, yes, giờ sửa thành clientroc à 
// trên lý thuyết thì sửa v nó sẽ ra chính xác
// nhưng nhờ cái lỗi này
// t mới ngộ ra là cho cái gridmap làm child của cái mapRenderer thì k thông minh lắm
// do có bnhiu client m f render bấy nhiêu lần cùng 1 thứ
// mà th cứ thử đê, nếu dc thì t chỉ cho cách mỗi client render 1 lần duy nhất th
// like this ? 
// RpcRender not ClientRender
// test xem
        [ClientRpc]
        private void RpcRender(int[] map, int width, int height)
        {
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
            Camera.main.GetComponent<CameraController>().SetMapData(botMap);
        }
    }
}
