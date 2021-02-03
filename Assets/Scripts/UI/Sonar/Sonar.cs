using MD.Character;
using MD.Diggable;
using MD.Diggable.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Functional;

namespace MD.UI
{
    public class Sonar : MonoBehaviour
    {
        private struct OutOfScannableRangeError : IError
        {
            public string Message => "Out of Scannable Range";
        }

        #region SERIALIZE FIELDS
        [SerializeField]
        private int scanRange = 3;

        [SerializeField]
        private RectTransform symbolContainer = null;

        [SerializeField]
        private GameObject symbolPoolObject = null;  
        #endregion

        #region FIELDS
        private Vector2[] relScannablePos;
        private Vector2 lastCenterPos = Vector2.zero;
        private float symbolSize;
        private IObjectPool symbolPool;
        private DiggableGeneratorCommunicator digGenComm;
        #endregion       

        void Start()
        {
            StartCoroutine(SetupOnDigGenCommInit());
        }

        private System.Collections.IEnumerator SetupOnDigGenCommInit()
        {
            bool initDigGenComm = false;

            while (!initDigGenComm)
            {
                ServiceLocator
                .Resolve<DiggableGeneratorCommunicator>()
                .Match(
                    unavailServiceErr => {},
                    digGenComm => 
                    {
                        this.digGenComm = digGenComm;
                        symbolPool = symbolPoolObject.GetComponent<IObjectPool>();
                        symbolSize = symbolContainer.rect.width / (2 * scanRange + 1);
                        relScannablePos = GenSquarePositions().ToArray();            
                        ListenToEvents();          
                        InitScanArea();

                        initDigGenComm = true;
                    }
                );

                yield return null;
            }
        }

        private void ListenToEvents()
        {
            var eventManager = EventSystems.EventManager.Instance;
            eventManager.StartListening<MoveData>(HandleMoveEvent);
            eventManager.StartListening<ScanData>(UpdateScanArea);
            eventManager.StartListening<DiggableSpawnData>(HandleDiggableSpawnEvent);
            eventManager.StartListening<DiggableRemoveData>(HandleDiggableRemoveEvent);
        }

        private void OnDestroy()
        {
            var eventManager = EventSystems.EventManager.Instance;
            eventManager.StopListening<MoveData>(HandleMoveEvent);
            eventManager.StopListening<ScanData>(UpdateScanArea);
            eventManager.StopListening<DiggableSpawnData>(HandleDiggableSpawnEvent);
            eventManager.StopListening<DiggableRemoveData>(HandleDiggableRemoveEvent);
        }

        #region UPDATE SCAN AREA
        private void InitScanArea()
        {
            if (!ServiceLocator.Resolve<Player>(out Player player)) { return; }

            RequestScanArea(player.transform.position.x, player.transform.position.y);
        }

        private void HandleMoveEvent(MoveData moveData)
        {
            float deltaX = lastCenterPos.x.DeltaInt(moveData.x);
            float deltaY = lastCenterPos.y.DeltaInt(moveData.y);

            if (deltaX <= Mathf.Epsilon && deltaY <= Mathf.Epsilon) return;

            lastCenterPos = new Vector2(moveData.x, moveData.y);
            (float roundedX, float roundedY) = (Mathf.Floor(moveData.x), Mathf.Floor(moveData.y));
            RequestScanArea(roundedX, roundedY);
        }

        private void RequestScanArea(float centerX, float centerY)
        {
            var scanArea = GetScannablePos(centerX, centerY).ToArray();
            digGenComm.CmdRequestScanArea(scanArea);
        }      

        private void UpdateScanArea(ScanData scanData) => Show(scanData.diggableArea);

        private void HandleDiggableSpawnEvent(DiggableSpawnData diggableSpawnData)
        {            
            ConvertWorldToScannablePos(diggableSpawnData.x, diggableSpawnData.y)
                .Match(
                    OutOfScannableRangeError => {},
                    scannablePos => PutSymbol(scannablePos, diggableSpawnData.type)
                );
        }

        private void PutSymbol(Vector2 pos, DiggableType diggableType)
        {
            var symbol = symbolPool.Pop().GetComponent<Image>();
            symbol.rectTransform.sizeDelta = new Vector2(symbolSize, symbolSize);
            symbol.transform.position = symbolContainer.position + new Vector3(pos.x * symbolSize, pos.y * symbolSize, 0f);           
            symbol.sprite = DiggableTypeConverter.Convert(diggableType).SonarSprite;
        }

        private void HandleDiggableRemoveEvent(DiggableRemoveData diggableRemoveData)
        {
            ConvertWorldToScannablePos(diggableRemoveData.x, diggableRemoveData.y)
                .Match(
                    outOfScannableRangeError => {},
                    scannablePos => RemoveSymbolAt(scannablePos.x, scannablePos.y)
                );
        }

        private Functional.Type.Either<OutOfScannableRangeError, Vector2> ConvertWorldToScannablePos(float x, float y)
        {
            Vector2 relPos = 
                new Vector2(
                    Mathf.Floor(x) - Mathf.Floor(lastCenterPos.x),
                    Mathf.Floor(y) - Mathf.Floor(lastCenterPos.y)
                );

            (Vector2 resPos, int idx) = relScannablePos.LookUp(pos => pos.x.IsEqual(relPos.x) && pos.y.IsEqual(relPos.y));

            if (idx.Equals(Constants.INVALID))
            {
                return new OutOfScannableRangeError();
            }

            return resPos;
        }    

        private void Show(DiggableType[] diggableArea)
        {
            symbolPool.Reset();
            
            for (int i = 0; i < diggableArea.Length; i++)
            {
                if (diggableArea[i].Equals(DiggableType.EMPTY)) continue;

                PutSymbol(relScannablePos[i], diggableArea[i]);
            }
        }

        private IEnumerable<Vector2Int> GetScannablePos(float charX, float charY)
        {
            foreach (var pos in relScannablePos)
            {
                yield return new Vector2Int(Mathf.FloorToInt(charX + pos.x), Mathf.FloorToInt(charY + pos.y));
            }
        }

        private void RemoveSymbolAt(float x, float y)
        {                          
            Vector2 pos = GetSymbolPos(x,y);
            (GameObject item, int idx) = symbolPool.LookUp(symbol => isSymbolAt(symbol,pos));

            if (idx.Equals(Constants.INVALID)) return;
            
            symbolPool.Push(item.gameObject);
        }

        private bool isSymbolAt(GameObject symbol, Vector2 pos)
        {
            if (Mathf.Abs(symbol.transform.position.x- pos.x) < .1f
             && Mathf.Abs(symbol.transform.position.y - pos.y) <.1f)
                return true;
            return false; 
        }

        private Vector2 GetSymbolPos(float x, float y)
        {
            return symbolContainer.position + new Vector3(x * symbolSize, y * symbolSize, 0f);
        }
        #endregion
                
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