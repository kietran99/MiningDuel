using Mirror;
using UnityEngine;
using System.Collections;
namespace MD.Diggable.Core
{
    public class FullMapSonar : NetworkBehaviour
    {
        public class FullMapSonarReady: NetworkMessage {}; 
        const short FULL_MAP_SONAR_CHANNEL = 1001; 
        private float GRID_MAP_OFFSET = .5f;

        #region SERIALIZE FIELDS
        [SerializeField]
        private GameObject tilePoolPrefab = null;

        [SerializeField]
        private Sprite emptyTileSprite = null;
        #endregion

        #region FIELDS
        private Transform playerTransform;
        private IObjectPool tilePool;
        private System.Collections.Generic.Dictionary<Vector2Int, SpriteRenderer> tileDataDict;
        #endregion

        private void Awake()
        {
            InitializePool();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            Debug.Log("im ready");
            NetworkClient.Send<FullMapSonarReady>(new FullMapSonarReady());
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler<FullMapSonarReady>(HandleReadymessage);
            SubscribeDiggableEvents();
        }

        [Server]
        private void HandleReadymessage(NetworkConnection conn, NetworkMessage mess)
        {
            SendScanData(conn);
        }


        [ServerCallback]
        private void OnDestroy()
        {
            UnsubscribeDiggableEvents();
        }

        private void InitializePool()
        {
            // GetComponent<SpriteMask>().enabled = true;
            // GetComponent<SpriteRenderer>().enabled = true;

            Debug.Log("Log Msg");
            ServiceLocator
                .Resolve<MD.Character.Player>()
                .Match(
                    err => Debug.LogError(err.Message),
                    player => 
                    {
                        playerTransform = player.transform;
                        tilePool = Instantiate(tilePoolPrefab).GetComponent<IObjectPool>();                                   
                        // CmdRequestScanData();   // Bug: something about NetworkWriter that can be fixed by modify the script then save it
                        // CmdSubscribeDiggableEvents();
                    }
                );
        }

        [Server]
        private void SendScanData(NetworkConnection conn)
        {
            ServiceLocator
                .Resolve<IDiggableGenerator>()
                .Match(
                    err => Debug.Log(err.Message),
                    diggableGenerator => 
                    {
                        TargetSetupSonarData(conn, diggableGenerator.InitSonarTileData);
                    }
                );
        }

        [TargetRpc]
        private void TargetSetupSonarData(NetworkConnection conn,SonarTileData[] sonarTileData)
        {

            Debug.Log("client rpc called");
            tileDataDict = new System.Collections.Generic.Dictionary<Vector2Int, SpriteRenderer>(sonarTileData.Length);

            sonarTileData
                .ForEach(
                    tileData =>
                    {    
                        var renderer = SetTileDataFromPool(tileData.x, tileData.y, tileData.type);                      
                        tileDataDict.Add(new Vector2Int(tileData.x, tileData.y), renderer);                       
                    }
                );
        }

        private SpriteRenderer SetTileDataFromPool(int x, int y, DiggableType type)
        {
            var tile = tilePool.Pop();
            tile.transform.position = new Vector3(x + GRID_MAP_OFFSET, y + GRID_MAP_OFFSET, 0f);
            var renderer = tile.GetComponentInChildren<SpriteRenderer>();
            renderer.sprite = GetSonarSprite(type);
            return renderer;
        }

        private Sprite GetSonarSprite(DiggableType type)
        {
            return type.Equals(DiggableType.EMPTY) ? emptyTileSprite : DiggableTypeConverter.Convert(type).SonarSprite;
        }

        [Server]
        private void SubscribeDiggableEvents()
        {
            ServiceLocator
                .Resolve<IDiggableGenerator>()
                .Match(
                    unavailServiceErr => Debug.LogError(unavailServiceErr.Message),
                    digGen => 
                    {
                        digGen.DiggableDestroyEvent     += RpcHandleDiggableDestroyEvent;
                        digGen.DiggableSpawnEvent       += RpcHandleDiggableSpawnEvent;
                    }
                );
        }

        [Server]
        private void UnsubscribeDiggableEvents()
        {
            ServiceLocator
                .Resolve<IDiggableGenerator>()
                .Match(
                    unavailServiceErr => Debug.LogError(unavailServiceErr.Message),
                    digGen => 
                    {
                        digGen.DiggableDestroyEvent     -= RpcHandleDiggableDestroyEvent;
                        digGen.DiggableSpawnEvent       -= RpcHandleDiggableSpawnEvent;
                    }
                );
        }

        [ClientRpc]
        private void RpcHandleDiggableDestroyEvent(DiggableRemoveData diggableRemoveData)
        {
            var spawnPos = new Vector2Int(diggableRemoveData.x, diggableRemoveData.y);

            if (tileDataDict.TryGetValue(spawnPos, out SpriteRenderer _))
            {
                tileDataDict[spawnPos].sprite = emptyTileSprite;
                return;
            }

            var renderer = SetTileDataFromPool(diggableRemoveData.x, diggableRemoveData.y, DiggableType.EMPTY);
            tileDataDict.Add(spawnPos, renderer);
        }

        [ClientRpc]
        private void RpcHandleDiggableSpawnEvent(DiggableSpawnData diggableSpawnData)
        {
            var spawnPos = new Vector2Int(diggableSpawnData.x, diggableSpawnData.y);
            var sprite = GetSonarSprite(diggableSpawnData.type);

            if (tileDataDict.TryGetValue(spawnPos, out SpriteRenderer _))
            {
                tileDataDict[spawnPos].sprite = sprite;
                return;
            }
            
            var renderer = SetTileDataFromPool(diggableSpawnData.x, diggableSpawnData.y, diggableSpawnData.type);
            tileDataDict.Add(spawnPos, renderer);
        }

        // private void LateUpdate()
        // {
        //     if (playerTransform == null) return;  

        //     transform.position = playerTransform.position;
        // }
    }
}
