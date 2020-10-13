using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Sonar : MonoBehaviour
{
    #region SERIALIZE FIELDS
    [SerializeField]
    private int scanRange = 3;

    [SerializeField]
    private RectTransform sonarImage = null;

    [SerializeField]
    private GameObject tilePoolObject = null;

    [SerializeField]
    private Image testObject = null;
    #endregion

    #region FIELDS
    private Vector2[] relativeScannablePos;
    private IMapManager mapManager;
    private IObjectPool tilePool;
    #endregion

    private List<Image> gems = new List<Image>();

    void Start()
    {
        tilePool = tilePoolObject.GetComponent<IObjectPool>();
        relativeScannablePos = GenerateScannablePositions(scanRange);
        EventSystems.EventManager.Instance.StartListening<MoveData>(UpdateScanArea);
        ServiceLocator.Resolve<IMapManager>(out mapManager);
        //Foo();
        Show(mapManager.GetScanAreaData(relativeScannablePos));
    }

    private void OnDestroy()
    {
        EventSystems.EventManager.Instance.StopListening<MoveData>(UpdateScanArea);
    }

    private void UpdateScanArea(MoveData moveData)
    {
        //tilePool.Reset();
        //scannablePos.Map(pos => tilePool.Pop().transform.position = 
        //new Vector3(moveData.x + pos.x, moveData.y + pos.y, 0f));
        Debug.Log(moveData.x + ", " + moveData.y);
        Vector2[] scanArea = GetScannablePos(Mathf.Floor(moveData.x), Mathf.Floor(moveData.y)).ToArray();
        Show(mapManager.GetScanAreaData(scanArea));
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
        gems.Map(_ => Destroy(_));
        gems.Clear();

        for (int i = 0; i < scanAreaData.Tiles.Length; i++)
        {
            if (scanAreaData[i].Diggable == 0) continue;

            Vector2 pos = scanAreaData[i].Position;
            Rect testRect = testObject.rectTransform.rect;
            gems.Add(Instantiate(testObject, sonarImage.position +
                new Vector3(pos.x * testRect.width, pos.y * testRect.height, 0f), Quaternion.identity, sonarImage));            
        }
    }

    private void Foo()
    {
        var scanAreaData = new ScanAreaData(Boo().ToArray());
        
        for (int i = 0; i < scanAreaData.Tiles.Length; i++)
        {
            if (scanAreaData[i].Diggable == 0) continue;

            var pos = scanAreaData[i].Position;
            var testRect = testObject.rectTransform.rect;
            Instantiate(testObject, sonarImage.position + 
                new Vector3(pos.x * testRect.width, pos.y * testRect.height, 0f), Quaternion.identity, sonarImage);
        }
    }

    private IEnumerable<ScanTileData> Boo()
    {
        foreach (var pos in relativeScannablePos)
        {
            yield return new ScanTileData(pos, UnityEngine.Random.Range(0, 2));
        }
    }

    #region SCANNABLE POSITIONS GENERATOR
    private Vector2[] GenerateScannablePositions(int scanRange)
    {
        var temp = new List<Vector2>();
        temp.AddRange(GenerateDiamondPositions(scanRange));
        temp.AddRange(GenerateFillerPositions(scanRange));
        return temp.ToArray();
    }

    private List<Vector2> GenerateDiamondPositions(int range)
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

    private List<Vector2> GenerateFillerPositions(int range)
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
