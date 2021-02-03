﻿using Mirror;
using UnityEngine;

namespace MD.Diggable.Core
{
    public class FullMapSonar : NetworkBehaviour
    {
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

        public override void OnStartAuthority()
        {
            ServiceLocator
                .Resolve<MD.Character.Player>()
                .Match(
                    err => Debug.LogError(err.Message),
                    player => 
                    {
                        playerTransform = player.transform;
                        ListenToEvents();   
                        tilePool = Instantiate(tilePoolPrefab).GetComponent<IObjectPool>();                                   
                        CmdRequestScanData();
                        CmdSubscribeDiggableEvents();
                    }
                );
        }

        public override void OnStopAuthority()
        {
            CmdUnsubscribeDiggableEvents();
        }

        private void ListenToEvents()
        {
            var eventConsumer = gameObject.AddComponent<EventSystems.EventConsumer>();
        }

        [Command]
        private void CmdRequestScanData()
        {
            ServiceLocator
                .Resolve<IDiggableGenerator>()
                .Match(
                    err => Debug.Log(err.Message),
                    diggableGenerator => TargetSetupSonarData(diggableGenerator.InitSonarTileData)
                );
        }

        [TargetRpc]
        private void TargetSetupSonarData(SonarTileData[] sonarTileData)
        {
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
            tile.transform.position = new Vector3(x, y, 0f);
            var renderer = tile.GetComponentInChildren<SpriteRenderer>();
            renderer.sprite = GetSonarSprite(type);
            return renderer;
        }

        private Sprite GetSonarSprite(DiggableType type)
        {
            return type.Equals(DiggableType.EMPTY) ? emptyTileSprite : DiggableTypeConverter.Convert(type).SonarSprite;
        }

        [Command]
        private void CmdSubscribeDiggableEvents()
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

        [Command]
        private void CmdUnsubscribeDiggableEvents()
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

        private void LateUpdate()
        {
            if (playerTransform == null) return;  

            transform.position = playerTransform.position;
        }
    }
}
