using UnityEngine;

public interface IMapManager
{
    ScanAreaData GetScanAreaData(Vector2[] tilesToScan);
}
