using MD.Diggable;
using MD.Diggable.Core;
using MD.Diggable.Gem;
using MD.Diggable.Projectile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    public class Sonar : MonoBehaviour
    {
        #region SERIALIZE FIELDS
        [SerializeField]
        private bool shouldShowDebugTiles = false;

        [SerializeField]
        private bool isTutorial = false;

        [SerializeField]
        private int scanRange = 3;

        [SerializeField]
        private RectTransform symbolContainer = null;

        [SerializeField]
        private GameObject symbolPoolObject = null;

        [SerializeField]
        private GameObject tilePoolObject = null;        
        #endregion

        #region FIELDS
        private Vector2[] relScannablePos;
        private IMapManager genManager;

        private IMapManager GenManager
        {
            get
            {
                if (genManager!= null) return genManager;
                ServiceLocator.Resolve<IMapManager>(out genManager);
                return genManager;
            }
        }

        private Vector2 lastCenterPos = Vector2.zero;
        private bool firstScan = true;
        private float symbolSize;
        private IObjectPool tilePool;
        private IObjectPool symbolPool;
        #endregion
        
        void Start()
        {
            symbolPool = symbolPoolObject.GetComponent<IObjectPool>();
            symbolSize = symbolContainer.rect.width / (2 * scanRange + 1);
            tilePool = tilePoolObject.GetComponent<IObjectPool>();
            relScannablePos = GenSquarePositions().ToArray();
            ListenToEvents();

            if (shouldShowDebugTiles)
            {
                relScannablePos.Map(pos => tilePool.Pop().transform.position
                = new Vector3(MapConstants.SPRITE_OFFSET.x + pos.x, MapConstants.SPRITE_OFFSET.y + pos.y, 0f));
            }

            // if (!isTutorial && genManager == null)
            // {
            //     ServiceLocator.Resolve(out genManager);
            // }

            if (genManager != null)
            {
                Show(GenManager.GetScanAreaData(relScannablePos));
            }
        }

        private void Show(ScanAreaData scanAreaData)
        {
            symbolPool.Reset();
            for (int i = 0; i < scanAreaData.Tiles.Length; i++)
            {
                // Debug.Log("pos " + scanAreaData[i].Position + " value:" + scanAreaData[i].Diggable);
                if (scanAreaData[i].Diggable == 0) continue;
                GenSymbol(relScannablePos[i], (DiggableType)scanAreaData[i].Diggable);
            }
        }

        private void Show(TileData[] tileData)
        {

        }

        private IEnumerable<TileData> ConvertToTileData(ScanTileData[] tileData)
        {
            for (int i = 0; i < tileData.Length; i++)
            {
                yield return new TileData((DiggableType)tileData[i].Diggable);
            }
        }

        private void GenSymbol(Vector2 pos, DiggableType diggableType)
        {
            var symbol = symbolPool.Pop().GetComponent<Image>();
            symbol.transform.position = symbolContainer.position + new Vector3(pos.x * symbolSize, pos.y * symbolSize, 0f);
            symbol.rectTransform.sizeDelta = new Vector2(symbolSize, symbolSize);
            symbol.sprite = DiggableTypeConverter.Convert(diggableType).SonarSprite;
        }

        public void BindScanAreaData(IMapManager genManager)
        {
            this.genManager = genManager;
        }

        private void ListenToEvents()
        {
            var eventManager = EventSystems.EventManager.Instance;
            eventManager.StartListening<MoveData>(AttemptToUpdateScanArea);
            // eventManager.StartListening<GemSpawnData>(UpdateScanArea);
            // eventManager.StartListening<GemDigSuccessData>(UpdateScanArea);
            // eventManager.StartListening<ProjectileObtainData>(UpdateScanArea);
        }

        private void OnDestroy()
        {
            var eventManager = EventSystems.EventManager.Instance;
            eventManager.StopListening<MoveData>(AttemptToUpdateScanArea);
            // eventManager.StopListening<GemSpawnData>(UpdateScanArea);
            // eventManager.StopListening<GemDigSuccessData>(UpdateScanArea);
            // eventManager.StopListening<ProjectileObtainData>(UpdateScanArea);
        }

        #region UPDATE SCAN AREA
        private void AttemptToUpdateScanArea(MoveData moveData)
        {
            if (firstScan)
            {
                firstScan = false;
                return;
            }

            float deltaX = lastCenterPos.x.DeltaInt(moveData.x);
            float deltaY = lastCenterPos.y.DeltaInt(moveData.y);

            if (deltaX <= Mathf.Epsilon && deltaY <= Mathf.Epsilon) return;

            lastCenterPos = new Vector2(moveData.x, moveData.y);
            (float roundedX, float roundedY) = //(Mathf.Floor(moveData.x), Mathf.Floor(moveData.y)); 
                (moveData.x.Round(), moveData.y.Round());
            MoveData roundedMoveData = new MoveData(roundedX, roundedY);
            UpdateScanArea(roundedMoveData);
        }

        private void UpdateScanArea(GemSpawnData gemSpawnData)
        {
            //Debug.Log("World position: " + gemSpawnData.x + ", " + gemSpawnData.y);
            if (!TryWorldToScannablePos(new Vector2(gemSpawnData.x, gemSpawnData.y), out Vector2 scannablePos)) return;

            //Debug.Log("Output: " + scannablePos);
            GenSymbol(scannablePos, gemSpawnData.type);
        }

        private bool TryWorldToScannablePos(Vector2 worldPos, out Vector2 scannablePos)
        {
            Vector2 relaPos = new Vector2(Mathf.Floor(worldPos.x - lastCenterPos.x),
                Mathf.Floor(worldPos.y - lastCenterPos.y));
            (Vector2 resPos, int idx) = relScannablePos.LookUp(
                pos => pos.x.IsEqual(relaPos.x) && pos.y.IsEqual(relaPos.y));
            scannablePos = resPos;
            return !idx.Equals(Constants.INVALID);
        }
                
        private void UpdateScanArea(GemDigSuccessData digSuccessData)
        {
            RemoveSymbolAtCentre();
        }

        private void UpdateScanArea(MoveData moveData)
        {
            //Debug.Log(moveData.x + ", " + moveData.y);
            if (shouldShowDebugTiles) ShowDebugArea(moveData);
            Vector2[] scanArea = GetScannablePos(moveData.x, moveData.y).ToArray();
            Show(GenManager.GetScanAreaData(scanArea));
        }

        private void UpdateScanArea(ProjectileObtainData obj)
        {
            RemoveSymbolAtCentre();
        }

        private void RemoveSymbolAtCentre()
        {                          
            (GameObject item, int idx) = symbolPool.LookUp(
                symbol => symbol.transform.position.x.IsEqual(symbolContainer.position.x) && 
                symbol.transform.position.y.IsEqual(symbolContainer.position.y));
            
            if (idx.Equals(Constants.INVALID)) return;

            symbolPool.Push(item.gameObject);
        }
        #endregion

        private void ShowDebugArea(MoveData moveData)
        {
            tilePool.Reset();
            relScannablePos.Map(pos => tilePool.Pop().transform.position =
            new Vector3(moveData.x + pos.x + MapConstants.SPRITE_OFFSET.x,
            moveData.y + pos.y + MapConstants.SPRITE_OFFSET.y, 0f));
        }

        private IEnumerable<Vector2> GetScannablePos(float charX, float charY)
        {
            foreach (var pos in relScannablePos)
            {
                yield return new Vector2(charX + pos.x, charY + pos.y);
            }
        }
                
        #region GENERATE SCANNABLE POSITIONS
        private IEnumerable<Vector2> GenSquarePositions()
        {
            var valueRange = Enumerable.Range(-scanRange, scanRange * 2 + 1);

            foreach (var x in valueRange)
            {
                foreach (var y in valueRange)
                {
                    yield return new Vector2(x, y);
                }
            }
        }

        private Vector2[] GenCircularScannablePositions(int scanRange)
        {
            var temp = new List<Vector2>();
            temp.AddRange(GenDiamondPositions(scanRange));
            temp.AddRange(GenFillerPositions(scanRange));
            return temp.ToArray();
        }

        private List<Vector2> GenDiamondPositions(int range)
        {
            var res = new List<Vector2>();

            for (int x = -range; x <= range; x++)
            {
                var yRange = range - Mathf.Abs(x);

                for (int y = -yRange; y <= yRange; y++)
                {
                    res.Add(new Vector2(x, y));
                }
            }

            return res;
        }

        private List<Vector2> GenFillerPositions(int range)
        {
            var res = new List<Vector2>();
            var fillerRange = range + 1;

            for (int x = -fillerRange; x <= fillerRange; x++)
            {
                var y = fillerRange - Mathf.Abs(x);

                if (y == 0 || y == fillerRange) continue;

                res.Add(new Vector2(x, y));
                res.Add(new Vector2(x, -y));
            }

            return res;
        }
        #endregion
    }
}