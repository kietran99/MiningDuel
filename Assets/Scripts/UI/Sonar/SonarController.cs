using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

namespace MD.Diggable.Core
{
    public class SonarController : NetworkBehaviour
    {
        #region SERIALIZE FIELDS
        [SerializeField]
        private int scannableSideLength = 7;

        [SerializeField]
        private GameObject tilePoolObject = null;
        #endregion

        #region FIELDS
        private Transform playerTransform;
        private Vector2Int[] relScannableArea;
        private Vector2 lastCenterPos;
        private IObjectPool tilePool;
        #endregion

        private void Start()
        {
            ServiceLocator
                .Resolve<MD.Character.Player>()
                .Match(
                    err => Debug.LogError(err.Message),
                    player => 
                    {
                        playerTransform = player.transform;
                        ListenToEvents();   
                        tilePool = tilePoolObject.GetComponent<IObjectPool>();
                        relScannableArea = GenRelativeSquareShape();                                           
                        RequestScanData();
                    }
                );
        }

        private void ListenToEvents()
        {
            var eventConsumer = gameObject.AddComponent<EventSystems.EventConsumer>();
        }

        private Vector2Int[] GenRelativeSquareShape()
        {
            var relScannablePos = new Vector2Int[scannableSideLength * scannableSideLength];
            var halfSideLen = scannableSideLength / 2;
            var cnt = 0;
            
            for (int i = -halfSideLen; i < halfSideLen + 1; i++)
            {
                for (int j = -halfSideLen; j < halfSideLen + 1; j++)
                {
                    relScannablePos[cnt++] = new Vector2Int(i, j);
                }
            }

            return relScannablePos;
        }
        
        private void RequestScanData()
        {
            CmdRequestScanArea(GetScannablePos().ToArray());  
        }

        private IEnumerable<Vector2Int> GetScannablePos()
        {
            foreach (var pos in relScannableArea)
            {
                yield return new Vector2Int(Mathf.FloorToInt(transform.position.x + pos.x), Mathf.FloorToInt(transform.position.y + pos.y));
            }
        }

        [Command]
        public void CmdRequestScanArea(Vector2Int[] positions)
        {
            ServiceLocator
                .Resolve<IDiggableGenerator>()
                .Match(
                    unavailServiceErr => Debug.LogError(unavailServiceErr.Message),
                    digGen => TargetUpdateScanArea(digGen.GetDiggableArea(positions))
                );
        }

        [TargetRpc]
        private void TargetUpdateScanArea(DiggableType[] scanArea)
        {
            tilePool.Reset();
            
            for (int i = 0; i < scanArea.Length; i++)
            {
                if (scanArea[i].Equals(DiggableType.EMPTY)) continue;

                var tile = tilePool.Pop();            
                
                tile.transform.localPosition = new Vector3(relScannableArea[i].x, relScannableArea[i].y, 0f);
                tile.GetComponentInChildren<SpriteRenderer>().sprite = DiggableTypeConverter.Convert(scanArea[i]).SonarSprite;
            }
        }

        private void LateUpdate()
        {
            if (playerTransform == null) return; 

            if (relScannableArea == null) return;       

            transform.position = playerTransform.position;
            RequestScanData();
        }
    }
}
