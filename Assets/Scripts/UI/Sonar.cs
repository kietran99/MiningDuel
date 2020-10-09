using System;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{
    [SerializeField]
    private int scanRange = 3;

    [SerializeField]
    private GameObject tilePoolObject = null;

    private Vector2[] scannablePos;
    private IObjectPool tilePool;

    void Start()
    {
        tilePool = tilePoolObject.GetComponent<IObjectPool>();
        scannablePos = GenerateScannablePositions(scanRange);
        EventSystems.EventManager.Instance.StartListening<MoveData>(UpdateScanArea);       
    }

    private void OnDestroy()
    {
        EventSystems.EventManager.Instance.StopListening<MoveData>(UpdateScanArea);
    }

    private void UpdateScanArea(MoveData moveData)
    {
        tilePool.Reset();
        scannablePos.Map(pos => tilePool.Pop().transform.position = 
        new Vector3(moveData.x + pos.x, moveData.y + pos.y, 0f));
    }

    private Vector2[] GenerateScannablePositions(int scanRange)
    {
        var temp = new List<Vector2>();
        temp.AddRange(GenerateDiamondPositions(scanRange));
        temp.AddRange(GenerateFillerPositions(scanRange));
        return temp.ToArray();
    }

    private List<Vector2> GenerateDiamondPositions(int range, Predicate<int> skipCond = null)
    {
        var res = new List<Vector2>();

        for (int x = -range; x <= range; x++)
        {
            var yRange = range - Mathf.Abs(x);

            if (skipCond != null && skipCond(yRange)) continue;

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
}
