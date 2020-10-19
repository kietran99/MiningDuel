using MD.Diggable;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    private RectTransform sonarImage = null;

    [SerializeField]
    private GameObject tilePoolObject = null;

    [SerializeField]
    private Image commonGemImage = null, uncommonGemImage = null, rareGemImage = null,
                    normalBombImage = null;
    #endregion

    #region FIELDS
    private Vector2[] relativeScannablePos;
    private IMapManager genManager;
    private Vector2 lastCenterPos = Vector2.zero;
    private bool firstScan = true;
    private IObjectPool tilePool;
    #endregion   

    private struct SonarSymbol
    {
        public float posX, posY;
        public Image symbol;

        public SonarSymbol(float posX, float posY, Image symbol)
        {
            this.posX = posX;
            this.posY = posY;
            this.symbol = symbol;
        }
    }

    private List<SonarSymbol> sonarSymbols = new List<SonarSymbol>();
    private Rect symbolSize;

    void Start()
    {
        symbolSize = commonGemImage.rectTransform.rect;
        tilePool = tilePoolObject.GetComponent<IObjectPool>();
        relativeScannablePos = GenScannablePositions(scanRange);
        ListenToEvents();
               
        if (shouldShowDebugTiles)
        {
            relativeScannablePos.Map(pos => tilePool.Pop().transform.position
            = new Vector3(MapConstants.SPRITE_OFFSET.x + pos.x, MapConstants.SPRITE_OFFSET.y + pos.y, 0f));
        }

        if (!isTutorial && genManager == null)
        {
            ServiceLocator.Resolve<IMapManager>(out genManager);           
            return;
        }

        if (genManager != null)
        {
            Show(genManager.GetScanAreaData(relativeScannablePos));
        }
    }

    public void BindScanAreaData(IMapManager genManager)
    {
        this.genManager = genManager;
    }

    private void ListenToEvents()
    {
        var eventManager = EventSystems.EventManager.Instance;
        eventManager.StartListening<MoveData>(AttemptToUpdateScanArea);
        eventManager.StartListening<GemSpawnData>(UpdateScanArea);
        eventManager.StartListening<GemDigSuccessData>(UpdateScanArea);
    }

    private void OnDestroy()
    {
        var eventManager = EventSystems.EventManager.Instance;
        eventManager.StopListening<MoveData>(UpdateScanArea);
        eventManager.StopListening<GemSpawnData>(UpdateScanArea);
        eventManager.StopListening<GemDigSuccessData>(UpdateScanArea);
    }

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
        (float roundedX, float roundedY) = (Mathf.Floor(moveData.x), Mathf.Floor(moveData.y)); 
        MoveData roundedMoveData = new MoveData(roundedX, roundedY);
        UpdateScanArea(roundedMoveData);
    }

    private void UpdateScanArea(GemSpawnData gemSpawnData)
    {
        //Debug.Log("World position: " + gemSpawnData.x + ", " + gemSpawnData.y);
        if (!TryWorldToScannablePos(new Vector2(gemSpawnData.x, gemSpawnData.y), out Vector2 scannablePos)) return;

        //Debug.Log("Output: " + scannablePos);
        sonarSymbols.Add(GenSonarSymbol(scannablePos.x, scannablePos.y, gemSpawnData.type));
    }
    
    private bool TryWorldToScannablePos(Vector2 worldPos, out Vector2 scannablePos)
    {
        Vector2 relaPos = new Vector2(Mathf.Floor(worldPos.x - lastCenterPos.x), 
            Mathf.Floor(worldPos.y - lastCenterPos.y));
        (Vector2 resPos, int idx) = relativeScannablePos.LookUp(
            pos => pos.x.IsEqual(relaPos.x) && pos.y.IsEqual(relaPos.y));
        scannablePos = resPos;
        return !idx.Equals(Constants.INVALID);
    }

    private SonarSymbol GenSonarSymbol(float relToCenterPosX, float relToCenterPosY, int gemValue)
    {
        float posOnSonarX = relToCenterPosX * symbolSize.width;
        float posOnSonarY = relToCenterPosY * symbolSize.height;
        Vector3 spawnPos = sonarImage.position + new Vector3(posOnSonarX, posOnSonarY, 0f);
        Image symbolImage = Instantiate(GetGemImage(gemValue), spawnPos, Quaternion.identity, sonarImage);
        return new SonarSymbol(relToCenterPosX, relToCenterPosY, symbolImage);
    }
   
    private Image GetGemImage(int gemValue)
    {
        switch (gemValue)
        {
            case 1: return commonGemImage; 
            case 4: return uncommonGemImage;
            case 10: return rareGemImage;
            case -1: return normalBombImage;
            default: return commonGemImage;
        }
    }

    private void UpdateScanArea(GemDigSuccessData digSuccessData)
    {
        (SonarSymbol gem, int idx) = sonarSymbols.ToArray().LookUp(_ =>_.posX.IsEqual(0f) && _.posY.IsEqual(0f));

        if (idx.Equals(Constants.INVALID)) return;

        Destroy(gem.symbol);
        sonarSymbols.Remove(gem);
    }
   
    private void UpdateScanArea(MoveData moveData)
    {       
        //Debug.Log(moveData.x + ", " + moveData.y);
        if (shouldShowDebugTiles) ShowDebugArea(moveData);
        Vector2[] scanArea = GetScannablePos(moveData.x, moveData.y).ToArray();
        Show(genManager.GetScanAreaData(scanArea));
    }

    private void ShowDebugArea(MoveData moveData)
    {
        tilePool.Reset();
        relativeScannablePos.Map(pos => tilePool.Pop().transform.position = 
        new Vector3(moveData.x + pos.x + MapConstants.SPRITE_OFFSET.x, 
        moveData.y + pos.y + MapConstants.SPRITE_OFFSET.y, 0f));        
    }

    private IEnumerable<Vector2> GetScannablePos(float charX, float charY)
    {
        foreach (var pos in relativeScannablePos)
        {
            yield return new Vector2(charX + pos.x, charY + pos.y);
        }
    }

    private void Show(ScanAreaData scanAreaData)
    {
        sonarSymbols.Map(_ => Destroy(_.symbol));
        sonarSymbols.Clear();
        
        for (int i = 0; i < scanAreaData.Tiles.Length; i++)
        {
            if (scanAreaData[i].Diggable == 0) continue;

            Vector2 pos = relativeScannablePos[i];
            sonarSymbols.Add(GenSonarSymbol(pos.x, pos.y, scanAreaData[i].Diggable));            
        }
    }
        
    #region GENERATE SCANNABLE POSITIONS
    private Vector2[] GenScannablePositions(int scanRange)
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
